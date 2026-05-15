using CardGame.Enums;
using CardGame.Games;
using CardGame.ViewModels;

namespace CardGame.GoFish;

public class GoFishPlugin : ICardGamePlugin
{
    public GameType GameType => GameType.GoFish;
    public string DisplayName => "Go Fish";
    public GameCategory Category => GameCategory.NonCasino;
    public bool IsAvailable => true;

    public ICardGame CreateGame(string playerName, Difficulty difficulty, int startingChips)
        => new GoFishGame(playerName, difficulty);

    public ViewModelBase CreateViewModel(ICardGame game, Action goBack, Func<int, Task> saveScore)
        => new GoFishViewModel((GoFishGame)game, goBack, saveScore);
}
