using PrimeTween;
using TMPro;
using UnityEngine;

public class CurrencyPanel : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float countDuration = 0.25f;
    [SerializeField] private float punchStrength = 0.1f;
    [SerializeField] private float punchDuration = 0.2f;

    [Header("References")]
    [SerializeField] private RectTransform panelTransform;
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