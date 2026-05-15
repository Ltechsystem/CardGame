using CardGame.Enums;
using CardGame.ViewModels;

namespace CardGame.Games;

public interface ICardGamePlugin
{
    GameType GameType { get; }
    string DisplayName { get; }
    GameCategory Category { get; }
    bool IsAvailable { get; }

    ICardGame CreateGame(string playerName, Difficulty difficulty, int startingChips);

    // saveScore: callback the plugin calls at session end with the base score.
    ViewModelBase CreateViewModel(ICardGame game, Action goBack, Func<int, Task> saveScore);
}
