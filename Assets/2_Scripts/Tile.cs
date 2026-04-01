using DNExtensions.Utilities;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private float visibleAlpha = 0.5f;
    [SerializeField] private Image overlayImage;
    
    private Tween _alphaTween;

    public void SetOverlay(bool visible)
    {
        if (!overlayImage) return;
        
        if (_alphaTween.isAlive) _alphaTween.Stop();
        
        var targetAlpha = visible ? visibleAlpha : 0f;
        overlayImage.color = overlayImage.color.SetAlpha(targetAlpha);
    }
    
    public Tween SetOverlayAnimated(bool visible)
    {
        if (!overlayImage) return new Tween();
        
        if (_alphaTween.isAlive) _alphaTween.Stop();
        
        var targetAlpha = visible ? visibleAlpha : 0f;
        _alphaTween = Tween.Alpha(overlayImage, targetAlpha, fadeDuration);
        return _alphaTween;
    }
}
