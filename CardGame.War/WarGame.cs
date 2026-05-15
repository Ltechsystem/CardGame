using CardGame.Enums;
using CardGame.Games;
using CardGame.Models;

namespace CardGame.War;

public class WarGame : ICardGame
{
    public readonly Player HumanPlayer;
    public readonly AiPlayer AiOpponent;
    private readonly Deck _deck = new();

    public GameType GameType => GameType.War;
    public Difficulty Difficulty => AiOpponent.Difficulty;
    public bool IsGameOver => _deck.IsEmpty && !HumanPlayer.Hand.Any() && !AiOpponent.Hand.Any();

    public WarGame(string playerName, Difficulty difficulty)
    {
        HumanPlayer = new Player(playerName);
        AiOpponent = new AiPlayer("Opponent", difficulty);
    }

    public void StartGame()
    {
        _deck.Shuffle();
        HumanPlayer.ClearHand();
        AiOpponent.ClearHand();
        HumanPlayer.AddToHand(_deck.Draw(26));
        AiOpponent.AddToHand(_deck.Draw(26));
    }

    // Returns winner of this battle (null = tie)
    public Player? PlayRound()
    {
        if (!HumanPlayer.Hand.Any() || !AiOpponent.Hand.Any()) return null;
        var pc = HumanPlayer.Hand[0];
        var ac = AiOpponent.Hand[0];
        HumanPlayer.RemoveFromHand(pc);
        AiOpponent.RemoveFromHand(ac);

        if (pc.Value > ac.Value) { HumanPlayer.Score++; return HumanPlayer; }
        if (ac.Value > pc.Value) { AiOpponent.Score++;  return AiOpponent; }
        return null;
    }

    public int GetBaseScore() => HumanPlayer.Score;

    public Player? GetWinner()
    {
        if (HumanPlayer.Score > AiOpponent.Score) return HumanPlayer;
        if (AiOpponent.Score > HumanPlayer.Score) return AiOpponent;
        return null;
    }
}
