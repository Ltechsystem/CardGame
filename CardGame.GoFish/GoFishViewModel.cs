using CardGame.Models;
using CardGame.ViewModels;

namespace CardGame.GoFish;

public class GoFishViewModel : ViewModelBase
{
    private readonly GoFishGame _game;
    private readonly Action _goBack;
    private readonly Func<int, Task> _saveScore;

    private bool _isResultPhase;
    private Card? _selectedCard;
    private string _actionLog = "Select a card from your hand and ask the AI for its rank!";
    private string _resultMessage = string.Empty;

    // ── Phase ────────────────────────────────────────────────────────────────
    public bool IsPlayingPhase => !_isResultPhase;
    public bool IsResultPhase  => _isResultPhase;

    // ── Scores & hands ───────────────────────────────────────────────────────
    public int PlayerSets        => _game.HumanPlayer.Score;
    public int AiSets            => _game.AiOpponent.Score;
    public int AiHandCount       => _game.AiOpponent.Hand.Count;
    public IReadOnlyList<Card> PlayerHand =>
        _game.HumanPlayer.Hand.OrderBy(c => (int)c.Rank).ToList();

    // ── Card selection ───────────────────────────────────────────────────────
    public Card? SelectedCard
    {
        get => _selectedCard;
        set
        {
            SetField(ref _selectedCard, value);
            AskCommand.RaiseCanExecuteChanged();
        }
    }

    // ── Messages ─────────────────────────────────────────────────────────────
    public string ActionLog
    {
        get => _actionLog;
        private set => SetField(ref _actionLog, value);
    }

    public string ResultMessage
    {
        get => _resultMessage;
        private set => SetField(ref _resultMessage, value);
    }

    // ── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand AskCommand        { get; }
    public RelayCommand BackToMenuCommand { get; }

    public GoFishViewModel(GoFishGame game, Action goBack, Func<int, Task> saveScore)
    {
        _game      = game;
        _goBack    = goBack;
        _saveScore = saveScore;

        _game.StartGame();

        AskCommand        = new RelayCommand(Ask, () => IsPlayingPhase && _selectedCard is not null);
        BackToMenuCommand = new RelayCommand(BackToMenu);
    }

    // ── Actions ──────────────────────────────────────────────────────────────
    private void Ask()
    {
        if (_selectedCard is null) return;

        var rank   = _selectedCard.Rank;
        var symbol = _selectedCard.RankSymbol;
        SelectedCard = null;

        var received = _game.HumanAsk(rank);

        string log;
        if (received.Count > 0)
        {
            log = $"You asked for {symbol}s and got {received.Count} card(s)! Go again!";
        }
        else
        {
            log = $"Go Fish! You drew a card.";
            if (!_game.IsGameOver)
            {
                var aiResult = _game.AiTurn();
                if (!string.IsNullOrEmpty(aiResult))
                    log += $"\n{aiResult}";
            }
        }

        ActionLog = log;
        Refresh();

        if (_game.IsGameOver) Resolve();
    }

    private void Resolve()
    {
        var winner = _game.GetWinner();
        ResultMessage = winner switch
        {
            null                               => $"It's a tie!  {PlayerSets} – {AiSets} sets",
            _ when winner == _game.HumanPlayer => $"You win!  {PlayerSets} – {AiSets} sets",
            _                                  => $"AI wins!  {AiSets} – {PlayerSets} sets"
        };
        _isResultPhase = true;
        Refresh();
    }

    private async void BackToMenu()
    {
        await _saveScore(_game.GetBaseScore());
        _goBack();
    }

    private void Refresh()
    {
        OnPropertyChanged(nameof(PlayerSets));
        OnPropertyChanged(nameof(AiSets));
        OnPropertyChanged(nameof(AiHandCount));
        OnPropertyChanged(nameof(PlayerHand));
        OnPropertyChanged(nameof(IsPlayingPhase));
        OnPropertyChanged(nameof(IsResultPhase));
        AskCommand.RaiseCanExecuteChanged();
    }
}
