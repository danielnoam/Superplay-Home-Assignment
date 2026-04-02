using DNExtensions.Systems.Scriptables;
using PrimeTween;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float iconDuration = 0.3f;
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private Ease iconEaseIn = Ease.OutBack;
    [SerializeField] private Ease iconEaseOut = Ease.InBack;

    [Header("References")]
    [SerializeField] private ParticleSystem glitterParticle;
    [SerializeField] private SOAudioEvent showSfx;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform iconTransform;
    
    private Sequence _sequence;
    private Vector3 _iconStartScale;

    private void Awake()
    {
        _iconStartScale = iconTransform.localScale;
        iconTransform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
    }


    public Sequence Show()
    {
        if (_sequence.isAlive) _sequence.Stop();

        _sequence = Sequence.Create()
            .ChainCallback(() =>
            {
                glitterParticle.Play();
                showSfx?.Play(audioSource);
            })
            .Group(Tween.Alpha(canvasGroup, 1, fadeDuration))
            .Group(Tween.Scale(iconTransform, _iconStartScale, iconDuration, iconEaseIn, startDelay: 0.2f))
            .ChainDelay(holdDuration)
            .Chain(Tween.Scale(iconTransform, Vector3.zero, iconDuration, iconEaseOut))
            .Group(Tween.Alpha(canvasGroup, 0f, fadeDuration, startDelay: 0.2f))
            .OnComplete(() =>
            {
                glitterParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            });
        
        return _sequence;
    }
}