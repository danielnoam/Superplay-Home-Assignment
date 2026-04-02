using System;
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
    [SerializeField] private float tileStaggerDelay = 0.1f;
    
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image text;
    [SerializeField] private Tile winingTile;
    [SerializeField] private Tile[] tiles = Array.Empty<Tile>();


    private Sequence _revealSequence;
    
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
            tile.SetOverlay(false);
        }
    }

    public Sequence AnimateHide()
    {
        if (_revealSequence.isAlive) _revealSequence.Stop();

        _revealSequence = Sequence.Create()
            .Group(Tween.Alpha(canvasGroup, 0, hideDuration, hideEase))
            .Group(Tween.Alpha(text, 0, hideDuration / 3, hideEase));
        
        return _revealSequence;
    }
    
    public Sequence AnimateReveal()
    {
        text.gameObject.SetActive(true);
        
        if (_revealSequence.isAlive) _revealSequence.Stop();
        
        _revealSequence = Sequence.Create()
            .Group(Tween.Alpha(canvasGroup, 1f, revealDuration, revealEase))
            .Group(Tween.Alpha(text, 1f, revealDuration / 3, revealEase));

        for (int i = 0; i < tiles.Length; i++)
        {
            float delay = i * tileStaggerDelay;
            _revealSequence.Group(tiles[i].ScaleUp(delay));
        }

        return _revealSequence;
    }
    
    [Button]
    private void FindAllTiles()
    {
        tiles = GetComponentsInChildren<Tile>();
    }
}