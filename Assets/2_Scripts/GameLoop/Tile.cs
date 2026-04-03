using System;
using DNExtensions.Systems.Scriptables;
using DNExtensions.Utilities;
using DNExtensions.Utilities.Button;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public static event Action<CanvasGroup, RectTransform> OnWin;
    
    [Header("Blink")]
    [SerializeField] private float blinkFadeDuration = 0.15f;
    [SerializeField] private float blinkPunchStrength = 0.1f;
    [SerializeField] private float blinkPunchDuration = 0.2f;
    [SerializeField] private float blinkAlpha = 0.5f;
    [SerializeField] private SOAudioEvent blinkSfx;
    
    [Header("Reveal")]
    [SerializeField] private float revealDuration = 0.15f;
    [SerializeField] private Ease revealEase = Ease.OutBack;
    [SerializeField] private SOAudioEvent revealSfx;
    
    [Header("Win")]
    [SerializeField] private float winPunchDuration = 0.2f;
    [SerializeField] private float winPunchStrength = 0.2f;
    [SerializeField] private float winVRevealDuration = 0.5f;
    [SerializeField] private Ease winVRevealEase = Ease.OutBack;
    [SerializeField] private SOAudioEvent winSfx;
    [SerializeField] private ParticleSystem winParticles;
    
    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Image overlayImage;
    [SerializeField] private Image vImage;
    [SerializeField] private Image shineImage;
    [SerializeField] private CanvasGroup prizeItem;
    [SerializeField] private RectTransform rectTransform;

    private bool _hadShine;
    private bool _initialized;
    private Vector3 _startScale;
    private Vector3 _vStartScale;
    
    private void Initialize()
    {
        _hadShine = shineImage.gameObject.activeSelf;
        _startScale = rectTransform.localScale;
        _vStartScale = vImage.transform.localScale;
        _initialized = true;
    }
    
    public void ResetTile()
    {
        if (!_initialized)
        {
            Initialize();
        }
        
        shineImage.gameObject.SetActive(_hadShine);
        prizeItem.gameObject.SetActive(true);
        vImage.transform.localScale = Vector3.zero;
        rectTransform.localScale = _startScale;
        overlayImage.color = overlayImage.color.SetAlpha(blinkAlpha);
    }
    
    public Sequence Win()
    {
        vImage.transform.localScale = Vector3.zero;
        
        var sequence = Sequence.Create()
            .ChainCallback(() =>
            {
                OnWin?.Invoke(prizeItem, rectTransform);
                shineImage.gameObject.SetActive(false);
                prizeItem.gameObject.SetActive(false);
                winSfx?.Play(audioSource);
                winParticles?.Play();
            })
            .Group(Tween.PunchScale(rectTransform, _startScale * winPunchStrength, winPunchDuration, 1))
            .Group(Tween.Alpha(overlayImage, 0, blinkFadeDuration))
            .Chain(Tween.Scale(vImage.transform, _vStartScale, winVRevealDuration, winVRevealEase)) ;
        
        return sequence;
    }
    
    public Sequence Blink(float startDelay = 0)
    {
        var sequence = Sequence.Create();
        if (startDelay > 0) sequence.ChainDelay(startDelay);
        
        sequence
            .ChainCallback(() => blinkSfx?.Play(audioSource))
            .Group(Tween.PunchScale(rectTransform, _startScale * blinkPunchStrength, blinkPunchDuration, 1))
            .Group(Tween.Alpha(overlayImage, 0, blinkFadeDuration/2))
            .Chain(Tween.Alpha(overlayImage, blinkAlpha, blinkFadeDuration/2));
        
        return sequence;
    }

    public Sequence AnimateReveal(float startDelay = 0)
    {
        rectTransform.localScale = Vector3.zero;
        
        var sequence = Sequence.Create();
        if (startDelay > 0) sequence.ChainDelay(startDelay);
        
        sequence
            .ChainCallback(() => revealSfx?.Play(audioSource))
            .Group(Tween.Scale(rectTransform, _startScale, revealDuration, revealEase));
        return sequence;
    }
}