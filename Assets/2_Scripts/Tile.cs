using System;
using DNExtensions.Utilities;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private float visibleAlpha = 0.5f;
    
    [Header("Scale")]
    [SerializeField] private float scaleDuration = 0.15f;

    [SerializeField] private Ease scaleEase = Ease.OutBack;
    
    [Header("References")]
    [SerializeField] private Image overlayImage;
    [SerializeField] private RectTransform rectTransform;
    
    private Tween _alphaTween;
    private Tween _scaleTween;
    private Vector3 _startScale;

    private void Awake()
    {
        _startScale = rectTransform.localScale;
    }

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
    
    public Tween ScaleUp(float startDelay = 0)
    {
        if  (!overlayImage) return new Tween();
        
        if (_alphaTween.isAlive) _alphaTween.Stop();
        rectTransform.localScale = Vector3.zero;
        
        _scaleTween = Tween.Scale(rectTransform, _startScale, scaleDuration, scaleEase, startDelay: startDelay);
        return _scaleTween;
    }
}
