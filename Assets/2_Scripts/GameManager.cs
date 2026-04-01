using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [SerializeField] private PrizeBoard[] boards = new PrizeBoard[3];
    
    [Header("References")]
    [SerializeField] private RectTransform boardFrame;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI playCostText;
    
    
    private Sequence _frameSequence;
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
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(StartRound);
    }
    
    private void StartNewGame()
    {
        _currency = 2000;
        currencyText.text = $"{_currency}";
        SetCurrentBoard(0);
    }
    
    private void SetCurrentBoard(int index)
    {
        if (_currentBoardIndex >= boards.Length - 1) return;
        
        var currentBoard = boards[_currentBoardIndex];
        if (currentBoard)
        {
            currentBoard.gameObject.SetActive(false);
        }
        
        _currentBoardIndex = index;
        var board = boards[_currentBoardIndex];
        
        
        boardFrame.offsetMin = new Vector2(board.BoardSize, boardFrame.offsetMin.y);
        boardFrame.offsetMax = new Vector2(-board.BoardSize, boardFrame.offsetMax.y);
        board.gameObject.SetActive(true);
        board.StartUpBoard();
        playCostText.text = $"{board.PlayCost}";
    }
    
    private void StartRound()
    {
        _currency -= boards[_currentBoardIndex].PlayCost;
        currencyText.text = $"{_currency}";
        
        SetCurrentBoard(_currentBoardIndex + 1);
    }
}
