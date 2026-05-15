using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CardGame.Services;
using CardGame.ViewModels;

namespace CardGame;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var registry = new GameRegistry();
            registry.LoadPlugins();

            var highscoreService = new HighscoreService();
            var balanceService   = new BalanceService();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(registry, highscoreService, balanceService)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}