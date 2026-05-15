using CardGame.Services;

namespace CardGame.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentPage;

    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        private set => SetField(ref _currentPage, value);
    }

    public MainWindowViewModel(GameRegistry registry, HighscoreService highscoreService, BalanceService balanceService)
    {
        _currentPage = new MainMenuViewModel(registry, highscoreService, balanceService, NavigateTo);
    }

    public void NavigateTo(ViewModelBase page) => CurrentPage = page;
}
