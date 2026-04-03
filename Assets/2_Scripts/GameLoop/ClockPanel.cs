using System;
using PrimeTween;
using TMPro;
using UnityEngine;

public class ClockPanel : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float punchStrength = 0.1f;
    [SerializeField] private float punchDuration = 0.2f;

    [Header("References")]
    [SerializeField] private RectTransform clockIcon;
    [SerializeField] private TextMeshProUGUI clockText;

    private Tween _punchTween;
    private int _remainingSeconds;
    private float _elapsed;

    private void Start()
    {
        _remainingSeconds = 86399;
        UpdateText();
    }

    private void Update()
    {
        if (_remainingSeconds <= 0) return;

        _elapsed += Time.deltaTime;
        if (_elapsed < 1f) return;

        var ticks = Mathf.FloorToInt(_elapsed);
        _elapsed -= ticks;
        _remainingSeconds = Mathf.Max(0, _remainingSeconds - ticks);

        UpdateText();

        if (_remainingSeconds % 10 == 0) PunchIcon();
    }

    private void UpdateText()
    {
        var time = TimeSpan.FromSeconds(_remainingSeconds);
        clockText.text = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
    }

    private void PunchIcon()
    {
        if (_punchTween.isAlive) _punchTween.Stop();
        _punchTween = Tween.PunchScale(clockIcon, Vector3.one * punchStrength, punchDuration);
    }
}