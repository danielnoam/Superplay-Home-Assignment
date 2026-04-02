using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct BoardSlot
{
    public Image image;
    public Sprite onSprite;
    public Sprite offSprite;
    public GameObject checkIcon;
}

public class ProgressBarPanel : MonoBehaviour
{
    [SerializeField] private float scaleDuration = 0.15f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    [SerializeField] private BoardSlot[] slots = Array.Empty<BoardSlot>();

    private void OnEnable()
    {
        GameManager.OnNewGame += HandleNewGame;
        GameManager.OnBoardChanged += HandleBoardChanged;
    }

    private void OnDisable()
    {
        GameManager.OnNewGame -= HandleNewGame;
        GameManager.OnBoardChanged -= HandleBoardChanged;
    }

    private void HandleNewGame(float _)
    {
        foreach (var slot in slots)
        {
            slot.image.sprite = slot.offSprite;
            slot.image.transform.localScale = Vector3.one;
            slot.checkIcon.SetActive(false);
        }
    }

    private void HandleBoardChanged(int currentIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            slot.checkIcon.SetActive(i < currentIndex);
            AnimateSlot(slot, i == currentIndex ? slot.onSprite : slot.offSprite);
        }
    }

    private void AnimateSlot(BoardSlot slot, Sprite targetSprite)
    {
        if (slot.image.sprite == targetSprite) return;

        var img = slot.image;
        Sequence.Create()
            .Chain(Tween.Scale(img.transform, 0f, scaleDuration, Ease.InBack))
            .ChainCallback(() => img.sprite = targetSprite)
            .Chain(Tween.Scale(img.transform, 1f, scaleDuration, scaleEase));
    }
}