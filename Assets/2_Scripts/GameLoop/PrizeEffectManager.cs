using System.Collections.Generic;
using DNExtensions.Utilities.Button;
using PrimeTween;
using TMPro;
using UnityEngine;

public class PrizeEffectManager : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float floatSpeed = 150f;
    [SerializeField] private float noiseStrength = 15f;
    [SerializeField] private float noiseFrequency = 4f;
    
    [Header("Scale")]
    [SerializeField] private float scaleUpDuration = 0.3f;
    [SerializeField] private float scaleUpStrength = 1.2f;
    [SerializeField] private Ease scaleUpEase = Ease.OutBack;
    
    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private float fadeDelay = 0.2f;
    
    [Header("References")]
    [SerializeField] private RectTransform effectHolder;

    private readonly List<ActiveEffect> _effects = new();

    private struct ActiveEffect
    {
        public RectTransform Rect;
        public CanvasGroup CanvasGroup;
        public float StartX;
        public float Age;
    }

    private void OnEnable() => Tile.OnWin += SpawnPrizeEffect;
    private void OnDisable() => Tile.OnWin -= SpawnPrizeEffect;

    private void Update()
    {
        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            var effect = _effects[i];
            
            if (effect.Rect == null)
            {
                _effects.RemoveAt(i);
                continue;
            }
            
            effect.Age += Time.deltaTime;
            _effects[i] = effect;

            var pos = effect.Rect.anchoredPosition;
            pos.y += floatSpeed * Time.deltaTime;
            pos.x = effect.StartX + Mathf.Sin(effect.Age * noiseFrequency * Mathf.PI * 2) * noiseStrength;
            effect.Rect.anchoredPosition = pos;
        }
    }

    private void SpawnPrizeEffect(CanvasGroup prizePrefab, RectTransform sourceRect)
    {
        var duplicate = Instantiate(prizePrefab, effectHolder);
        var dupRect = duplicate.GetComponent<RectTransform>();
        var dupText = duplicate.GetComponentInChildren<TextMeshProUGUI>(true);
        
        duplicate.alpha = 1f;
        dupRect.position = sourceRect.position;
        dupRect.localScale = Vector3.zero;
        dupText.gameObject.SetActive(true);
        duplicate.gameObject.SetActive(true);

        var startX = dupRect.anchoredPosition.x;

        Tween.Scale(dupRect, scaleUpStrength, scaleUpDuration, scaleUpEase);
        Tween.Alpha(duplicate, 0f, fadeDuration, Ease.InQuad, startDelay: fadeDelay)
            .OnComplete(() => Destroy(duplicate.gameObject));

        _effects.Add(new ActiveEffect
        {
            Rect = dupRect,
            CanvasGroup = duplicate,
            StartX = startX,
            Age = 0f
        });
    }
}