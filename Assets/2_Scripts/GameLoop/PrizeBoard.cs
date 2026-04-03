using System;
using System.Collections.Generic;
using DNExtensions.Utilities;
using DNExtensions.Utilities.Button;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class PrizeBoard : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int playCost = 1000;
    [SerializeField] private int randomTileCount = 4;
    [SerializeField] private float boardSize;
    
    [Header("Animation")]
    [SerializeField] private float revealDuration = 1f;
    [SerializeField] private Ease revealEase = Ease.Linear;
    [SerializeField] private float hideDuration = 0.5f;
    [SerializeField] private Ease hideEase = Ease.Linear;
    [SerializeField] private float revelStaggerDelay = 0.1f;
    
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image text;
    [SerializeField] private Tile winingTile;
    [SerializeField] private Tile[] tiles = Array.Empty<Tile>();
    
    
    public int PlayCost => playCost;
    public float BoardSize => boardSize;
    public Tile WiningTile => winingTile;
    public Tile[] Tiles => tiles;
    public CanvasGroup CanvasGroup => canvasGroup;
    
    
    
    public void ResetBoard()
    {
        canvasGroup.alpha = 0f;
        text.gameObject.SetActive(false);
        text.color = text.color.SetAlpha(0f);
        
        foreach (var tile in tiles)
        {
            tile.ResetTile();
        }
    }

    public Sequence AnimateWin()
    {
        var candidates = new List<Tile>(tiles);
        candidates.Remove(winingTile);
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        var sequence = Sequence.Create();
    
        int count = Mathf.Min(randomTileCount, candidates.Count);
        for (int i = 0; i < count; i++)
        {
            sequence.Chain(candidates[i].Blink());
        }
        
        sequence
            .ChainDelay(0.5f)
            .Chain(winingTile.Win());

        return sequence;
    }

    public Sequence AnimateHide()
    {
        var sequence = Sequence.Create()
            .Group(Tween.Alpha(canvasGroup, 0, hideDuration, hideEase))
            .Group(Tween.Alpha(text, 0, hideDuration / 3, hideEase));
        
        return sequence;
    }
    
    public Sequence AnimateReveal()
    {
        text.gameObject.SetActive(true);
    
        var sequence = Sequence.Create()
            .Group(Tween.Alpha(canvasGroup, 1f, revealDuration, revealEase))
            .Group(Tween.Alpha(text, 1f, revealDuration / 3, revealEase));

        var shuffled = new List<Tile>(tiles);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        for (int i = 0; i < shuffled.Count; i++)
        {
            float delay = i * revelStaggerDelay;
            sequence.Group(shuffled[i].AnimateReveal(delay));
        }
        
        for (int i = 0; i < shuffled.Count; i++)
        {
            float delay = i * revelStaggerDelay;
            if (i == 0)
            {
                sequence.Chain(shuffled[i].Blink());
            }
            else
            {
                sequence.Group(shuffled[i].Blink(delay));
            }

        }

        return sequence;
    }
    
    [Button]
    private void FindAllTiles()
    {
        tiles = GetComponentsInChildren<Tile>();
    }
}