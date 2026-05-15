using CardGame.Enums;
using CardGame.Models;

namespace CardGame.Games;

public interface ICardGame
{
    GameType GameType { get; }
    Difficulty Difficulty { get; }

    void StartGame();
    bool IsGameOver { get; }

    int GetBaseScore();
    Player? GetWinner();
}
