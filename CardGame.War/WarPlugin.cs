using CardGame.Enums;
using CardGame.Games;
using CardGame.ViewModels;

namespace CardGame.War;

public class WarPlugin : ICardGamePlugin
{
    public GameType GameType => GameType.War;
    public string DisplayName => "War";
    public GameCategory Category => GameCategory.NonCasino;
    public bool IsAvailable => true;

    public ICardGame CreateGame(string playerName, Difficulty difficulty, int startingChips)
        => new WarGame(playerName, difficulty);

    public ViewModelBase CreateViewModel(ICardGame game, Action goBack, Func<int, Task> saveScore)
        => new WarViewModel((WarGame)game, goBack, saveScore);
}
