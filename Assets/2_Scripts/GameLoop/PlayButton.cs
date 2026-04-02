using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease hideEase = Ease.InBack;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private TextMeshProUGUI costText;

    private Button _button;
    private Tween _tween;

    public Button Button => _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetInteractable(bool interactable, bool animated)
    {
        if (_button.interactable == interactable) return;
        
        if (_tween.isAlive) _tween.Stop();

        _button.interactable = interactable;

        if (animated)
        {
            _tween = interactable ? Tween.ScaleX(transform, 1f, animationDuration, showEase) : Tween.ScaleX(transform, 0f, animationDuration, hideEase);
        }
        else
        {
            transform.localScale = new Vector3(interactable ? 1f : 0f, transform.localScale.y, transform.localScale.z);
        }
        
    }

    public void SetCostText(int cost)
    {
        costText.text = $"{cost}";
    }
}