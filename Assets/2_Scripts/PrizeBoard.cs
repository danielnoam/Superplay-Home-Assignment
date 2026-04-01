using System;
using DNExtensions.Utilities.Button;
using UnityEngine;

public class PrizeBoard : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int playCost = 1000;
    [SerializeField] private float boardSize;
    
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Tile winingTile;
    [SerializeField] private Tile[] tiles = Array.Empty<Tile>();


    public int PlayCost => playCost;
    public float BoardSize => boardSize;
    public Tile WiningTile => winingTile;
    public Tile[] Tiles => tiles;

    
    [Button]
    private void FindAllTiles()
    {
        tiles = GetComponentsInChildren<Tile>();
    }
    
    public void StartUpBoard()
    {
        foreach (var tile in tiles)
        {
            tile.SetOverlay(false);
        }
        
        canvasGroup.alpha = 1;
    }
}