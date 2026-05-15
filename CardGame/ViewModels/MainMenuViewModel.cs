using CardGame.Config;
using CardGame.Enums;
using CardGame.Games;
using CardGame.Services;

namespace CardGame.ViewModels;

public record GameOption(ICardGamePlugin Plugin)
{
    public string DisplayName => Plugin.DisplayName;
}

public record DifficultyOption(Difficulty Difficulty, string DisplayName);

public class MainMenuViewModel : ViewModelBase
{
    private readonly GameRegistry _registry;
    private readonly HighscoreService _highscoreService;
    private readonly BalanceService _balanceService;
    private readonly Action<ViewModelBase> _navigate;

    private string _playerName = string.Empty;
    private GameOption? _selectedCasinoGame;
    private GameOption? _selectedNonCasinoGame;
    private DifficultyOption _selectedDifficulty;
    private int _chipBalance;

    // ── Game lists ───────────────────────────────────────────────────────────
    public List<GameOption> CasinoGames    { get; }
    public List<GameOption> NonCasinoGames { get; }

    public bool HasNoGames       => CasinoGames.Count == 0 && NonCasinoGames.Count == 0;
    public bool HasCasinoGames   => CasinoGames.Count > 0;
    public bool HasNonCasinoGames => NonCasinoGames.Count > 0;

    // ── Chips ────────────────────────────────────────────────────────────────
    public int ChipBalance
    {
        get => _chipBalance;
        private set => SetField(ref _chipBalance, value);
    }

    // ── Difficulty ───────────────────────────────────────────────────────────
    public List<DifficultyOption> AvailableDifficulties { get; } =
    [
        new(Difficulty.Easy,   "Easy"),
        new(Difficulty.Medium, "Medium"),
        new(Difficulty.Hard,   "Hard")
    ];

    public DifficultyOption SelectedDifficulty
    {
        get => _selectedDifficulty;
        set => SetField(ref _selectedDifficulty, value);
    }

    // ── Player name ──────────────────────────────────────────────────────────
    public string PlayerName
    {
        get => _playerName;
        set
        {
            SetField(ref _playerName, value);
            StartGameCommand.RaiseCanExecuteChanged();
            LoadBalanceAsync();
        }
    }

    // ── Selection — selecting one group clears the other ─────────────────────
    public GameOption? SelectedCasinoGame
    {
        get => _selectedCasinoGame;
        set
        {
            SetField(ref _selectedCasinoGame, value);
            if (value is not null) SelectedNonCasinoGame = null;
            StartGameCommand.RaiseCanExecuteChanged();
        }
    }

    public GameOption? SelectedNonCasinoGame
    {
        get => _selectedNonCasinoGame;
        set
        {
            SetField(ref _selectedNonCasinoGame, value);
            if (value is not null) SelectedCasinoGame = null;
            StartGameCommand.RaiseCanExecuteChanged();
        }
    }

    private GameOption? SelectedGame => _selectedCasinoGame ?? _selectedNonCasinoGame;

    // ── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand StartGameCommand      { get; }
    public RelayCommand ViewHighscoresCommand { get; }

    public MainMenuViewModel(
        GameRegistry registry,
        HighscoreService highscoreService,
        BalanceService balanceService,
        Action<ViewModelBase> navigate,
        string initialPlayerName = "")
    {
        _registry         = registry;
        _highscoreService = highscoreService;
        _balanceService   = balanceService;
        _navigate         = navigate;

        var available = registry.Plugins.Where(p => p.IsAvailable).ToList();
        CasinoGames    = available.Where(p => p.Category == GameCategory.Casino).Select(p => new GameOption(p)).ToList();
        NonCasinoGames = available.Where(p => p.Category == GameCategory.NonCasino).Select(p => new GameOption(p)).ToList();

        _selectedCasinoGame  = CasinoGames.FirstOrDefault();
        _selectedDifficulty  = AvailableDifficulties[1]; // Medium default

        StartGameCommand      = new RelayCommand(StartGame, CanStartGame);
        ViewHighscoresCommand = new RelayCommand(ViewHighscores);

        if (!string.IsNullOrWhiteSpace(initialPlayerName))
        {
            _playerName = initialPlayerName;
            LoadBalanceAsync();
        }
    }

    // ── Balance loading ──────────────────────────────────────────────────────
    private async void LoadBalanceAsync()
    {
        if (string.IsNullOrWhiteSpace(PlayerName)) return;
        ChipBalance = await _balanceService.GetBalanceAsync(PlayerName);
    }

    // ── Game start ───────────────────────────────────────────────────────────
    private bool CanStartGame() => !string.IsNullOrWhiteSpace(PlayerName) && SelectedGame is not null;

    private async void StartGame()
    {
        var plugin = SelectedGame?.Plugin;
        if (plugin is null) return;

        var difficulty    = SelectedDifficulty.Difficulty;
        int startingChips = await _balanceService.GetBalanceAsync(PlayerName);
        var game          = plugin.CreateGame(PlayerName, difficulty, startingChips);

        async Task SaveSession(int baseScore)
        {
            await _highscoreService.SaveScoreAsync(PlayerName, game.GameType, difficulty, baseScore);

            int finalChips;
            if (plugin.Category == GameCategory.Casino)
            {
                // For casino games the base score IS the chip balance.
                finalChips = baseScore;
            }
            else
            {
                // Award chips for winning non-casino sessions.
                var winner    = game.GetWinner();
                bool playerWon = winner?.Name == PlayerName;
                int reward    = playerWon ? (int)(50 * DifficultySettings.GetMultiplier(difficulty)) : 0;
                finalChips    = startingChips + reward;
            }

            ChipBalance = finalChips;
            await _balanceService.SetBalanceAsync(PlayerName, finalChips);
        }

        var vm = plugin.CreateViewModel(game, GoBack, SaveSession);
        _navigate(vm);
    }

    private void GoBack() =>
        _navigate(new MainMenuViewModel(_registry, _highscoreService, _balanceService, _navigate, PlayerName));

    private void ViewHighscores()
    {
        // Wired up when HighscoreViewModel is built.
    }
}
