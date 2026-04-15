using SocOps.Models;

namespace SocOps.Services;

public class GameManager
{
    private readonly BingoGameService _bingoService;
    private readonly ScavengerHuntService _scavengerService;

    public GameMode CurrentMode { get; private set; } = GameMode.Bingo;

    public GameState CurrentGameState => CurrentMode == GameMode.Bingo ? _bingoService.CurrentGameState : _scavengerService.CurrentGameState;
    public List<BingoSquareData> Board => CurrentMode == GameMode.Bingo ? _bingoService.Board : _scavengerService.Checklist;
    public BingoLine? WinningLine => CurrentMode == GameMode.Bingo ? _bingoService.WinningLine : null;
    public HashSet<int> WinningSquareIds => CurrentMode == GameMode.Bingo ? _bingoService.WinningSquareIds : new HashSet<int>();
    public bool ShowModal => CurrentMode == GameMode.Bingo ? _bingoService.ShowBingoModal : _scavengerService.ShowCompletionModal;

    public event Action? OnStateChanged;

    public GameManager(BingoGameService bingoService, ScavengerHuntService scavengerService)
    {
        _bingoService = bingoService;
        _scavengerService = scavengerService;
        _bingoService.OnStateChanged += () => OnStateChanged?.Invoke();
        _scavengerService.OnStateChanged += () => OnStateChanged?.Invoke();
    }

    public void SetMode(GameMode mode)
    {
        CurrentMode = mode;
        OnStateChanged?.Invoke();
    }

    public async Task InitializeAsync()
    {
        await _bingoService.InitializeAsync();
        await _scavengerService.InitializeAsync();
    }

    public async Task StartGame()
    {
        if (CurrentMode == GameMode.Bingo)
            await _bingoService.StartGame();
        else
            await _scavengerService.StartGame();
    }

    public async Task HandleSquareClick(int squareId)
    {
        if (CurrentMode == GameMode.Bingo)
            await _bingoService.HandleSquareClick(squareId);
        else
            await _scavengerService.HandleItemClick(squareId);
    }

    public async Task ResetGame()
    {
        if (CurrentMode == GameMode.Bingo)
            await _bingoService.ResetGame();
        else
            await _scavengerService.ResetGame();
    }

    public void DismissModal()
    {
        if (CurrentMode == GameMode.Bingo)
            _bingoService.DismissModal();
        else
            _scavengerService.DismissModal();
    }
}