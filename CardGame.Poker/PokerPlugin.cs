using CardGame.Enums;
using CardGame.Games;
using CardGame.ViewModels;

namespace CardGame.Poker;

public class PokerPlugin : ICardGamePlugin
{
    public GameType GameType => GameType.Poker;
    public string DisplayName => "Poker";
    public GameCategory Category => GameCategory.Casino;
    public bool IsAvailable => false;

    public ICardGame CreateGame(string playerName, Difficulty difficulty, int startingChips)
        => new PokerGame(playerName, difficulty);

    public ViewModelBase CreateViewModel(ICardGame game, Action goBack, Func<int, Task> saveScore)
        => throw new NotImplementedException("Poker UI not yet implemented.");
}
