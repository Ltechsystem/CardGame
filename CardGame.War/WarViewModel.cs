using CardGame.Models;
using CardGame.ViewModels;

namespace CardGame.War;

public class WarViewModel : ViewModelBase
{
    private readonly WarGame _game;
    private readonly Action _goBack;
    private readonly Func<int, Task> _saveScore;

    private bool _isResultPhase;
    private bool _hasPlayedRound;
    private Card? _playerCard;
    private Card? _aiCard;
    private string _roundResult = "Click DRAW CARD to play a round";
    private string _resultMessage = string.Empty;

    // ── Phase ────────────────────────────────────────────────────────────────
    public bool IsPlayingPhase => !_isResultPhase;
    public bool IsResultPhase  => _isResultPhase;

    // ── Scores & cards left ──────────────────────────────────────────────────
    public int PlayerWins => _game.HumanPlayer.Score;
    public int AiWins     => _game.AiOpponent.Score;
    public int CardsLeft  => _game.HumanPlayer.Hand.Count;

    // ── Last played cards (flattened for compiled bindings) ──────────────────
    public bool HasPlayedRound
    {
        get => _hasPlayedRound;
        private set => SetField(ref _hasPlayedRound, value);
    }

    public string PlayerCardRank  => _playerCard?.RankSymbol  ?? "";
    public string PlayerCardSuite => _playerCard?.SuiteSymbol ?? "";
    public bool   PlayerCardIsRed => _playerCard?.IsRed       ?? false;

    public string AiCardRank      => _aiCard?.RankSymbol  ?? "";
    public string AiCardSuite     => _aiCard?.SuiteSymbol ?? "";
    public bool   AiCardIsRed     => _aiCard?.IsRed       ?? false;

    // ── Messages ─────────────────────────────────────────────────────────────
    public string RoundResult
    {
        get => _roundResult;
        private set => SetField(ref _roundResult, value);
    }

    public string ResultMessage
    {
        get => _resultMessage;
        private set => SetField(ref _resultMessage, value);
    }

    // ── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand DrawCommand       { get; }
    public RelayCommand BackToMenuCommand { get; }

    public WarViewModel(WarGame game, Action goBack, Func<int, Task> saveScore)
    {
        _game      = game;
        _goBack    = goBack;
        _saveScore = saveScore;

        _game.StartGame();

        DrawCommand       = new RelayCommand(Draw,      () => IsPlayingPhase);
        BackToMenuCommand = new RelayCommand(BackToMenu);
    }

    // ── Actions ──────────────────────────────────────────────────────────────
    private void Draw()
    {
        // Capture cards before PlayRound removes them from the hands
        var playerTop = _game.HumanPlayer.Hand[0];
        var aiTop     = _game.AiOpponent.Hand[0];

        var roundWinner = _game.PlayRound();

        _playerCard = playerTop;
        _aiCard     = aiTop;
        _hasPlayedRound = true;

        RoundResult = roundWinner switch
        {
            null                                 => "Tie!",
            _ when roundWinner == _game.HumanPlayer => "You win the round!",
            _                                    => "AI wins the round!"
        };

        Refresh();

        if (_game.IsGameOver) Resolve();
    }

    private void Resolve()
    {
        var winner = _game.GetWinner();

        ResultMessage = winner switch
        {
            null                               => $"It's a tie!  {PlayerWins} – {AiWins}",
            _ when winner == _game.HumanPlayer => $"You win!  {PlayerWins} – {AiWins}",
            _                                  => $"AI wins!  {AiWins} – {PlayerWins}"
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
        OnPropertyChanged(nameof(PlayerWins));
        OnPropertyChanged(nameof(AiWins));
        OnPropertyChanged(nameof(CardsLeft));
        OnPropertyChanged(nameof(IsPlayingPhase));
        OnPropertyChanged(nameof(IsResultPhase));
        OnPropertyChanged(nameof(PlayerCardRank));
        OnPropertyChanged(nameof(PlayerCardSuite));
        OnPropertyChanged(nameof(PlayerCardIsRed));
        OnPropertyChanged(nameof(AiCardRank));
        OnPropertyChanged(nameof(AiCardSuite));
        OnPropertyChanged(nameof(AiCardIsRed));
        DrawCommand.RaiseCanExecuteChanged();
    }
}
