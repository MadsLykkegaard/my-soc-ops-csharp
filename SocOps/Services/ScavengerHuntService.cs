using SocOps.Models;
using SocOps.Data;
using System.Text.Json;
using Microsoft.JSInterop;

namespace SocOps.Services;

public class ScavengerHuntService
{
    private const string STORAGE_KEY = "scavenger-hunt-state";
    private const int STORAGE_VERSION = 1;

    private readonly IJSRuntime _jsRuntime;

    public GameState CurrentGameState { get; private set; } = GameState.Start;
    public List<BingoSquareData> Checklist { get; private set; } = new();
    public bool ShowCompletionModal { get; private set; }

    public event Action? OnStateChanged;

    public ScavengerHuntService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        await LoadGameStateAsync();
    }

    public async Task StartGame()
    {
        Checklist = GenerateChecklist();
        CurrentGameState = GameState.Playing;
        ShowCompletionModal = false;
        await SaveGameStateAsync();
        NotifyStateChanged();
    }

    public async Task HandleItemClick(int itemId)
    {
        var item = Checklist.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            item.IsMarked = !item.IsMarked;
        }

        // Check for completion
        if (Checklist.All(i => i.IsMarked))
        {
            CurrentGameState = GameState.Bingo;
            ShowCompletionModal = true;
        }

        await SaveGameStateAsync();
        NotifyStateChanged();
    }

    public async Task ResetGame()
    {
        CurrentGameState = GameState.Start;
        Checklist = new();
        ShowCompletionModal = false;
        await SaveGameStateAsync();
        NotifyStateChanged();
    }

    public void DismissModal()
    {
        ShowCompletionModal = false;
        NotifyStateChanged();
    }

    private List<BingoSquareData> GenerateChecklist()
    {
        var shuffled = Questions.QuestionsList.OrderBy(_ => Guid.NewGuid()).ToList();
        return shuffled.Select((q, i) => new BingoSquareData
        {
            Id = i,
            Text = q,
            IsMarked = false,
            IsFreeSpace = false
        }).ToList();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();

    private async Task LoadGameStateAsync()
    {
        try
        {
            var saved = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", STORAGE_KEY);
            if (!string.IsNullOrEmpty(saved))
            {
                var data = JsonSerializer.Deserialize<StoredGameData>(saved);
                if (data != null && data.Version == STORAGE_VERSION)
                {
                    CurrentGameState = data.GameState;
                    Checklist = data.Checklist;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load game state: {ex.Message}");
        }
    }

    private async Task SaveGameStateAsync()
    {
        try
        {
            var data = new StoredGameData
            {
                Version = STORAGE_VERSION,
                GameState = CurrentGameState,
                Checklist = Checklist
            };
            var json = JsonSerializer.Serialize(data);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save game state: {ex.Message}");
        }
    }

    private class StoredGameData
    {
        public int Version { get; set; }
        public GameState GameState { get; set; }
        public List<BingoSquareData> Checklist { get; set; } = new();
    }
}