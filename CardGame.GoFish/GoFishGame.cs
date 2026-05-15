using CardGame.Enums;
using CardGame.Games;
using CardGame.Models;

namespace CardGame.GoFish;

public class GoFishGame : ICardGame
{
    public readonly Player HumanPlayer;
    public readonly AiPlayer AiOpponent;
    private readonly Deck _deck = new();
    private readonly Random _rng = new();

    public GameType GameType => GameType.GoFish;
    public Difficulty Difficulty => AiOpponent.Difficulty;
    public bool IsGameOver => _deck.IsEmpty && !HumanPlayer.Hand.Any() && !AiOpponent.Hand.Any();

    public GoFishGame(string playerName, Difficulty difficulty)
    {
        HumanPlayer = new Player(playerName);
        AiOpponent = new AiPlayer("Opponent", difficulty);
    }

    public void StartGame()
    {
        _deck.Shuffle();
        HumanPlayer.ClearHand();
        AiOpponent.ClearHand();
        HumanPlayer.Score = 0;
        AiOpponent.Score = 0;
        HumanPlayer.AddToHand(_deck.Draw(7));
        AiOpponent.AddToHand(_deck.Draw(7));
        CheckAndScoreSets(HumanPlayer);
        CheckAndScoreSets(AiOpponent);
    }

    // Human asks the AI for a rank. Must hold that rank. Returns cards received (empty = "Go Fish").
    public List<Card> HumanAsk(PlayingCardNo rank)
    {
        var cards = AiOpponent.RemoveAllOfRank(rank);
        if (cards.Count > 0)
        {
            HumanPlayer.AddToHand(cards);
        }
        else
        {
            var drawn = _deck.Draw();
            if (drawn is not null) HumanPlayer.AddToHand(drawn);
        }
        CheckAndScoreSets(HumanPlayer);
        return cards;
    }

    // AI takes its turn and returns a description of what happened.
    public string AiTurn()
    {
        if (!AiOpponent.Hand.Any()) return string.Empty;

        var rank = ChooseAiRank();
        var cards = HumanPlayer.RemoveAllOfRank(rank);
        string result;
        if (cards.Count > 0)
        {
            AiOpponent.AddToHand(cards);
            result = $"Opponent asked for {rank}s and got {cards.Count} card(s).";
        }
        else
        {
            var drawn = _deck.Draw();
            if (drawn is not null) AiOpponent.AddToHand(drawn);
            result = $"Opponent asked for {rank}s — Go Fish!";
        }
        CheckAndScoreSets(AiOpponent);
        return result;
    }

    private PlayingCardNo ChooseAiRank()
    {
        var available = AiOpponent.Hand.Select(c => c.Rank).Distinct().ToList();
        return Difficulty switch
        {
            Difficulty.Easy => available[_rng.Next(available.Count)],
            _ => available
                .OrderByDescending(r => AiOpponent.Hand.Count(c => c.Rank == r))
                .First()
        };
    }

    private static void CheckAndScoreSets(Player player)
    {
        foreach (PlayingCardNo rank in Enum.GetValues<PlayingCardNo>())
        {
            if (player.Hand.Count(c => c.Rank == rank) == 4)
            {
                player.RemoveAllOfRank(rank);
                player.Score++;
            }
        }
    }

    public int GetBaseScore() => HumanPlayer.Score;

    public Player? GetWinner()
    {
        if (HumanPlayer.Score > AiOpponent.Score) return HumanPlayer;
        if (AiOpponent.Score > HumanPlayer.Score) return AiOpponent;
        return null;
    }
}
