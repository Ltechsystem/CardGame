using CardGame.Enums;
using CardGame.Games;
using CardGame.Models;

namespace CardGame.Poker;

// Texas Hold'em skeleton — full hand-ranking logic implemented in the Poker build step.
public class PokerGame : ICardGame
{
    public readonly Player HumanPlayer;
    public readonly AiPlayer AiOpponent;
    private readonly Deck _deck = new();

    public List<Card> CommunityCards { get; } = [];

    public GameType GameType => GameType.Poker;
    public Difficulty Difficulty => AiOpponent.Difficulty;
    public bool IsGameOver { get; private set; }

    public PokerGame(string playerName, Difficulty difficulty)
    {
        HumanPlayer = new Player(playerName);
        AiOpponent = new AiPlayer("Opponent", difficulty);
    }

    public void StartGame()
    {
        _deck.Shuffle();
        HumanPlayer.ClearHand();
        AiOpponent.ClearHand();
        CommunityCards.Clear();
        HumanPlayer.AddToHand(_deck.Draw(2));
        AiOpponent.AddToHand(_deck.Draw(2));
        IsGameOver = false;
    }

    public void DealFlop()  => CommunityCards.AddRange(_deck.Draw(3));
    public void DealTurn()  => CommunityCards.AddRange(_deck.Draw(1));
    public void DealRiver() { CommunityCards.AddRange(_deck.Draw(1)); IsGameOver = true; }

    public int GetBaseScore() => HumanPlayer.Score;

    public Player? GetWinner() => null; // hand-ranking logic added in Poker build step
}
