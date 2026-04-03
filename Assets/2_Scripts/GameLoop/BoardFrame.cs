using DNExtensions.Systems.Scriptables;
using PrimeTween;
using UnityEngine;

public class BoardFrame : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease ease = Ease.OutBack;
    
    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private SOAudioEvent sizeSfx;
    [SerializeField] private AudioSource audioSource;


    public void ResetFrame()
    {
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    public Sequence AnimateToSize(float boardSize)
    {
        var targetMin = new Vector2(boardSize, rectTransform.offsetMin.y);
        var targetMax = new Vector2(-boardSize, rectTransform.offsetMax.y);

        if (Mathf.Approximately(rectTransform.offsetMin.x, targetMin.x) && Mathf.Approximately(rectTransform.offsetMax.x, targetMax.x)) return Sequence.Create();

        return Sequence.Create()
            .ChainCallback(() => sizeSfx?.Play(audioSource))
            .Group(Tween.UIOffsetMin(rectTransform, targetMin, duration, ease))
            .Group(Tween.UIOffsetMax(rectTransform, targetMax, duration, ease));
    }
}