using CardGame.Enums;

namespace CardGame.Models;

public class Player
{
    public string Name { get; }
    public List<Card> Hand { get; } = [];
    public int Score { get; set; }

    public Player(string name)
    {
        Name = name;
    }

    public void AddToHand(Card card) => Hand.Add(card);
    public void AddToHand(IEnumerable<Card> cards) => Hand.AddRange(cards);
    public bool RemoveFromHand(Card card) => Hand.Remove(card);

    public List<Card> RemoveAllOfRank(PlayingCardNo rank)
    {
        var matches = Hand.Where(c => c.Rank == rank).ToList();
        foreach (var card in matches) Hand.Remove(card);
        return matches;
    }

    public bool HasRank(PlayingCardNo rank) => Hand.Any(c => c.Rank == rank);
    public void ClearHand() => Hand.Clear();
}
