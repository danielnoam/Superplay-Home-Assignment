using System;
using PrimeTween;
using TMPro;
using UnityEngine;

public class CurrencyPanel : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float countDuration = 0.25f;
    [SerializeField] private float punchStrength = 0.1f;
    [SerializeField] private float punchDuration = 0.2f;
    
    [Header("Icon")]
    [SerializeField] private float noiseStrength = 15f;
    [SerializeField] private float noiseFrequency = 4f;
    
    [Header("References")]
    [SerializeField] private RectTransform panelTransform;
    [SerializeField] private RectTransform currencyIcon;
    [SerializeField] private TextMeshProUGUI currencyText;

    private Tween _countTween;
    private Tween _punchTween;
    private float _displayedValue;
    
    private void OnEnable()
    {
        GameManager.OnNewGame += HandleNewGame;
        GameManager.OnCurrencyChanged += HandleCurrencyChanged;
    }

    private void OnDisable()
    {
        GameManager.OnNewGame -= HandleNewGame;
        GameManager.OnCurrencyChanged -= HandleCurrencyChanged;
    }

    private void Update()
    {
        var noise = Mathf.PerlinNoise(Time.time * noiseFrequency, 0f) - 0.5f;
        currencyIcon.localRotation = Quaternion.Euler(0f, 0f, noise * noiseStrength);
    }

    private void HandleNewGame(float value)
    {
        _displayedValue = value;
        currencyText.text = $"{(int)value}";
    }

    private void HandleCurrencyChanged(float value)
    {
        var from = _displayedValue;
        _displayedValue = value;

        if (_countTween.isAlive) _countTween.Stop();
        _countTween = Tween.Custom(this, from, value, countDuration, static (self, val) =>
        {
            self.currencyText.text = $"{(int)val}";
        });

        if (_punchTween.isAlive) _punchTween.Stop();
        _punchTween = Tween.PunchScale(panelTransform, Vector3.one * punchStrength, punchDuration);
    }
}