using CardGame.Enums;
using CardGame.Games;
using CardGame.ViewModels;

namespace CardGame.Blackjack;

public class BlackjackPlugin : ICardGamePlugin
{
    public GameType GameType => GameType.Blackjack;
    public string DisplayName => "Blackjack";
    public GameCategory Category => GameCategory.Casino;
    public bool IsAvailable => true;

    public ICardGame CreateGame(string playerName, Difficulty difficulty, int startingChips)
        => new BlackjackGame(playerName, difficulty, startingChips);

    public ViewModelBase CreateViewModel(ICardGame game, Action goBack, Func<int, Task> saveScore)
        => new BlackjackViewModel((IBlackjackGame)game, goBack, saveScore);
}
