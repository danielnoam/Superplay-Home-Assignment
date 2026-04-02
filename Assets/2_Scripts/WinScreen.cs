using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float iconDuration = 0.3f;
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private Ease iconEaseIn = Ease.OutBack;
    [SerializeField] private Ease iconEaseOut = Ease.InBack;
    
    [Header("Glow & Glitter")]
    [SerializeField] private float glitterRotationSpeed = 45f;
    [SerializeField] private float glowFrequency = 2f;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform iconTransform;
    [SerializeField] private Image glowImage;
    [SerializeField] private Image glitterImage;

    private bool _isVisible;
    private Sequence _sequence;
    private Vector3 _iconStartScale;

    private void Awake()
    {
        _isVisible = false;
        _iconStartScale = iconTransform.localScale;
        iconTransform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
    }
    
    private void Update()
    {
        if (!_isVisible) return;

        glitterImage.transform.Rotate(0f, 0f, glitterRotationSpeed * Time.deltaTime);
        glowImage.color = new Color(1f, 1f, 1f, (Mathf.Sin(Time.time * glowFrequency * Mathf.PI * 2f) + 1f) * 0.5f);
    }

    public void Show(Action onComplete = null)
    {
        if (_sequence.isAlive) _sequence.Stop();
        
        _isVisible = true;

        _sequence = Sequence.Create()
            .Group(Tween.Alpha(canvasGroup, 1, fadeDuration))
            .Group(Tween.Scale(iconTransform, _iconStartScale, iconDuration, iconEaseIn, startDelay: 0.2f))
            .ChainDelay(holdDuration)
            .Chain(Tween.Scale(iconTransform, Vector3.zero, iconDuration, iconEaseOut))
            .Group(Tween.Alpha(canvasGroup, 0f, fadeDuration, startDelay: 0.2f));

        if (onComplete != null) _sequence.OnComplete(onComplete);
    }
}