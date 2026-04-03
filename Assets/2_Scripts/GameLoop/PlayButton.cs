using DNExtensions.Systems.Scriptables;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease hideEase = Ease.InBack;
    [SerializeField] private Ease showEase = Ease.OutBack;

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private SOAudioEvent scaleUpSfx;
    [SerializeField] private SOAudioEvent scaleDownSfx;

    private Tween _tween;
    public Button Button => button;
    

    public void TurnOn()
    {
        if (_tween.isAlive) _tween.Stop();
        
        button.interactable = true;
        scaleUpSfx?.Play(audioSource);
        _tween = Tween.ScaleX(transform, 1f, animationDuration, showEase);
    }

    public void TurnOff(bool animated)
    {
        if (_tween.isAlive) _tween.Stop();
        
        button.interactable = false;

        if (animated)
        {
            scaleDownSfx?.Play(audioSource);
            _tween = Tween.ScaleX(transform, 0f, animationDuration, hideEase);
        }
        else
        {
            transform.localScale = new Vector3(0f, transform.localScale.y, transform.localScale.z);
        }

        
    }

    public void SetCostText(int cost)
    {
        costText.text = $"{cost}";
    }
}