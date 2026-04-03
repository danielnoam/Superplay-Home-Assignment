using System;
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
    
    [Header("References")]
    [SerializeField] private Button exitButton;
    [SerializeField] private PlayButton playButton;
    [SerializeField] private WinScreen winScreen;
    [SerializeField] private BoardFrame boardFrame;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) StartNewGame();
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
        
        playButton.Button.onClick.RemoveAllListeners();
        playButton.Button.onClick.AddListener(StartRound);
    }
    
    private void StartNewGame()
    {
        if (_animationSequence.isAlive) _animationSequence.Stop();
        
        _currency = initialCurrency;
        playButton.TurnOff(false);
        foreach (var prizeBoard in boards)
        {
            prizeBoard.ResetBoard();
        }
        boardFrame.ResetFrame();
        
        OnNewGame?.Invoke(_currency);
        
        SetCurrentBoard(0);
    }
    
    private void SetCurrentBoard(int index)
    {
        if (_currentBoardIndex >= boards.Length - 1) return;
    
        if (_animationSequence.isAlive) _animationSequence.Stop();
        _animationSequence = Sequence.Create();
        
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
    
        _currentBoardIndex = index;
        OnBoardChanged?.Invoke(_currentBoardIndex);
        var prizeBoard = boards[_currentBoardIndex];
        prizeBoard.gameObject.SetActive(true);
        
        _animationSequence
            .Group(boardFrame.AnimateToSize(prizeBoard.BoardSize))
            .Chain(prizeBoard.AnimateReveal())
            .OnComplete(() =>
            {
                if (boards.Length > _currentBoardIndex)
                {
                    playButton.SetCostText(boards[_currentBoardIndex].PlayCost);
                    playButton.TurnOn();
                }
            });
    }
    
    private void StartRound()
    {
        _currency -= boards[_currentBoardIndex].PlayCost;
        OnCurrencyChanged?.Invoke(_currency);
        playButton.TurnOff(true);

        var board = boards[_currentBoardIndex];
        _animationSequence = Sequence.Create()
            .Chain(board.AnimateWin())
            .ChainDelay(1f)
            .ChainCallback(() => winScreen.PlayWinScreen())
            .ChainDelay(winScreen.TimelineDuration)
            .ChainCallback(() => SetCurrentBoard(_currentBoardIndex + 1));
    }
}