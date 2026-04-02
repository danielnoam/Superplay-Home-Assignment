using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = PrimeTween.Sequence;


public class GameManager : MonoBehaviour
{
    public static event Action<float> OnNewGame;
    public static event Action<float> OnCurrencyChanged;
    public static event Action<int> OnBoardChanged;
    
    [Header("Settings")]
    [SerializeField] private int initialCurrency = 3000;
    
    [Header("Frame Change Animation")]
    [SerializeField] private float frameAnimationDuration = 0.25f;
    [SerializeField] private Ease frameAnimationEase = Ease.OutBack;
    
    [Header("References")]
    [SerializeField] private Button exitButton;
    [SerializeField] private RectTransform boardFrame;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private WinScreen winScreen;
    [SerializeField] private PrizeBoard[] boards = new PrizeBoard[3];
    
    
    private Sequence _animationSequence;
    private int _currentBoardIndex;
    private int _currency;
    

    private void Awake()
    {
        SetUp();
    }

    private void Start()
    {
        StartNewGame();
    }
    
    private void SetUp()
    {
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener((() =>
        {
            #region UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            return;
            #endregion
            Application.Quit();
        }));
        
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(StartRound);
    }
    
    private void StartNewGame()
    {
        _currency = initialCurrency;
        playButton.interactable = false;
        foreach (var board in boards)
        {
            board.ResetBoard();
        }
        
        OnNewGame?.Invoke(_currency);
        
        SetCurrentBoard(0);

    }
    
    private void SetCurrentBoard(int index)
    {
        if (_currentBoardIndex >= boards.Length - 1) return;
    
        // start new sequence
        if (_animationSequence.isAlive) _animationSequence.Stop();
        _animationSequence = Sequence.Create();
        
        // disable old board
        if (index != _currentBoardIndex)
        {
            var previousBoard = boards[_currentBoardIndex];
            _animationSequence
                .Group(previousBoard.AnimateHide())
                .ChainCallback(() =>
                {
                    previousBoard.ResetBoard();
                    previousBoard.gameObject.SetActive(false);
                });
        }
    
        // update to new board
        _currentBoardIndex = index;
        OnBoardChanged?.Invoke(_currentBoardIndex);
        var board = boards[_currentBoardIndex];
        board.gameObject.SetActive(true);

        // animate new board in
        if (!Mathf.Approximately(boardFrame.anchorMin.x, board.BoardSize) || !Mathf.Approximately(boardFrame.anchorMax.x, 1 - board.BoardSize))
        {
            _animationSequence
                .Group(Tween.UIOffsetMin(boardFrame, new Vector2(board.BoardSize, boardFrame.offsetMin.y), frameAnimationDuration, frameAnimationEase))
                .Group(Tween.UIOffsetMax(boardFrame, new Vector2(-board.BoardSize, boardFrame.offsetMax.y), frameAnimationDuration, frameAnimationEase));
        }
        _animationSequence
            .Chain(board.AnimateReveal())
            .Chain(Tween.Custom(this, 0f, 1f, 0.01f, static (_, _) => { })
                .OnComplete(this, static self => self.playCostText.text = $"{self.boards[self._currentBoardIndex].PlayCost}"))
            .OnComplete(() =>
            {
                if (boards.Length > _currentBoardIndex)
                {
                    playCostText.text = $"{boards[_currentBoardIndex].PlayCost}";
                    playButton.interactable = true;
                }
            });
    }
    
    private void StartRound()
    {
        _currency -= boards[_currentBoardIndex].PlayCost;
        OnCurrencyChanged?.Invoke(_currency);
        playButton.interactable = false;

        var board = boards[_currentBoardIndex];
        _animationSequence = Sequence.Create()
            .Chain(board.AnimateWin())
            .ChainDelay(1f)
            .Chain(winScreen.Show())
            .ChainCallback(() => SetCurrentBoard(_currentBoardIndex + 1));
    }
}
