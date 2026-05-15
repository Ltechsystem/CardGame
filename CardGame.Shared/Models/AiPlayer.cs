using CardGame.Enums;

namespace CardGame.Models;

// Base AI player — game-specific decision logic lives in each game's own assembly.
public class AiPlayer : Player
{
    public Difficulty Difficulty { get; }

    public AiPlayer(string name, Difficulty difficulty) : base(name)
    {
        Difficulty = difficulty;
    }
}
