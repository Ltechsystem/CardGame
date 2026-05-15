using CardGame.Games;
using CardGame.Models;
using CardGame.ViewModels;

namespace CardGame.Blackjack;

public class BlackjackViewModel : ViewModelBase
{
    private readonly IBlackjackGame _game;
    private readonly Action _goBack;
    private readonly Func<int, Task> _saveScore;

    private enum Phase { Betting, Playing, Result }
    private Phase _phase = Phase.Betting;

    private string _betInput = "100";
    private string _statusMessage = "Place your bet";
    private string _resultMessage = string.Empty;
    private bool _dealerCardHidden = true;

    // ── Phase flags ──────────────────────────────────────────────────────────
    public bool IsBettingPhase => _phase == Phase.Betting;
    public bool IsPlayingPhase => _phase == Phase.Playing;
    public bool IsResultPhase  => _phase == Phase.Result;
    public bool IsBankrupt     => _phase == Phase.Result && _game.ChipBalance == 0;

    // ── Chip & bet ───────────────────────────────────────────────────────────
    public int ChipBalance => _game.ChipBalance;

    public string BetInput
    {
        get => _betInput;
        set { SetField(ref _betInput, value); DealCommand.RaiseCanExecuteChanged(); }
    }

    private int ParsedBet => int.TryParse(BetInput, out var v) ? v : 0;

    // ── Hands ────────────────────────────────────────────────────────────────
    public IReadOnlyList<Card> PlayerHand => _game.HumanPlayer.Hand;

    public IReadOnlyList<Card> DealerVisibleHand => _dealerCardHidden && _game.Dealer.Hand.Count > 1
        ? _game.Dealer.Hand.Take(1).ToList()
        : _game.Dealer.Hand;

    public bool HasHiddenDealerCard => _dealerCardHidden && _game.Dealer.Hand.Count > 1;

    public int PlayerHandValue     => BlackjackRules.HandValue(_game.HumanPlayer.Hand);
    public int DealerVisibleValue  => BlackjackRules.HandValue(DealerVisibleHand);
    public int DealerHandValue     => BlackjackRules.HandValue(_game.Dealer.Hand);

    // ── Messages ─────────────────────────────────────────────────────────────
    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public string ResultMessage
    {
        get => _resultMessage;
        private set => SetField(ref _resultMessage, value);
    }

    // ── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand DealCommand       { get; }
    public RelayCommand Bet25Command      { get; }
    public RelayCommand Bet50Command      { get; }
    public RelayCommand Bet100Command     { get; }
    public RelayCommand BetAllInCommand   { get; }
    public RelayCommand HitCommand        { get; }
    public RelayCommand StandCommand      { get; }
    public RelayCommand NextHandCommand   { get; }
    public RelayCommand BackToMenuCommand { get; }

    public BlackjackViewModel(IBlackjackGame game, Action goBack, Func<int, Task> saveScore)
    {
        _game      = game;
        _goBack    = goBack;
        _saveScore = saveScore;

        DealCommand     = new RelayCommand(Deal,  () => ParsedBet > 0 && ParsedBet <= _game.ChipBalance);
        Bet25Command    = new RelayCommand(() => BetInput = "25",                      () => _game.ChipBalance >= 25);
        Bet50Command    = new RelayCommand(() => BetInput = "50",                      () => _game.ChipBalance >= 50);
        Bet100Command   = new RelayCommand(() => BetInput = "100",                     () => _game.ChipBalance >= 100);
        BetAllInCommand = new RelayCommand(() => BetInput = _game.ChipBalance.ToString());
        HitCommand      = new RelayCommand(Hit,       () => _phase == Phase.Playing);
        StandCommand    = new RelayCommand(Stand,     () => _phase == Phase.Playing);
        NextHandCommand = new RelayCommand(NextHand,  () => _phase == Phase.Result && !IsBankrupt);
        BackToMenuCommand = new RelayCommand(BackToMenu);
    }

    // ── Actions ──────────────────────────────────────────────────────────────
    private void Deal()
    {
        if (!_game.PlaceBet(ParsedBet)) return;
        _game.StartGame();
        _dealerCardHidden = true;
        _phase = Phase.Playing;
        StatusMessage = "Your turn";

        if (_game.IsGameOver) Resolve(); // natural blackjack resolved immediately
        else Refresh();
    }

    private void Hit()
    {
        _game.PlayerHit();
        if (_game.IsGameOver) Resolve();
        else Refresh();
    }

    private void Stand()
    {
        _game.PlayerStand();
        Resolve();
    }

    private void Resolve()
    {
        _dealerCardHidden = false;
        var winner  = _game.GetWinner();
        int net     = _game.SettleBet();
        bool natural = BlackjackRules.IsNatural(_game.HumanPlayer.Hand) &&
                       !BlackjackRules.IsNatural(_game.Dealer.Hand);

        StatusMessage = winner switch
        {
            null                               => "Push!",
            _ when winner == _game.HumanPlayer => natural ? "Blackjack!" : "You win!",
            _                                  => "Dealer wins!"
        };

        ResultMessage = net > 0 ? $"+{net} chips"
                      : net < 0 ? $"{net} chips"
                      : "Bet returned";

        _phase = Phase.Result;
        Refresh();
    }

    private void NextHand()
    {
        _phase = Phase.Betting;
        StatusMessage = "Place your bet";
        ResultMessage = string.Empty;
        Refresh();
    }

    private async void BackToMenu()
    {
        await _saveScore(_game.GetBaseScore());
        _goBack();
    }

    private void Refresh()
    {
        OnPropertyChanged(nameof(ChipBalance));
        OnPropertyChanged(nameof(PlayerHand));
        OnPropertyChanged(nameof(DealerVisibleHand));
        OnPropertyChanged(nameof(HasHiddenDealerCard));
        OnPropertyChanged(nameof(PlayerHandValue));
        OnPropertyChanged(nameof(DealerVisibleValue));
        OnPropertyChanged(nameof(DealerHandValue));
        OnPropertyChanged(nameof(IsBettingPhase));
        OnPropertyChanged(nameof(IsPlayingPhase));
        OnPropertyChanged(nameof(IsResultPhase));
        OnPropertyChanged(nameof(IsBankrupt));
        DealCommand.RaiseCanExecuteChanged();
        Bet25Command.RaiseCanExecuteChanged();
        Bet50Command.RaiseCanExecuteChanged();
        Bet100Command.RaiseCanExecuteChanged();
        HitCommand.RaiseCanExecuteChanged();
        StandCommand.RaiseCanExecuteChanged();
        NextHandCommand.RaiseCanExecuteChanged();
    }
}
