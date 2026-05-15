using CardGame.Enums;

namespace CardGame.Models;

public class Deck
{
    private readonly List<Card> _cards = [];

    public int CardsRemaining => _cards.Count;
    public bool IsEmpty => _cards.Count == 0;

    public Deck()
    {
        foreach (PlayingCardSuite suite in Enum.GetValues<PlayingCardSuite>())
        foreach (PlayingCardNo rank in Enum.GetValues<PlayingCardNo>())
            _cards.Add(new Card(rank, suite));
    }

    // Restores all 52 cards so the same Deck instance can be reused across rounds.
    public void Reset()
    {
        _cards.Clear();
        foreach (PlayingCardSuite suite in Enum.GetValues<PlayingCardSuite>())
        foreach (PlayingCardNo rank in Enum.GetValues<PlayingCardNo>())
            _cards.Add(new Card(rank, suite));
    }

    public void Shuffle()
    {
        var rng = new Random();
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public Card? Draw()
    {
        if (IsEmpty) return null;
        var card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }

    public List<Card> Draw(int count)
    {
        var drawn = new List<Card>();
        for (int i = 0; i < count && !IsEmpty; i++)
            drawn.Add(Draw()!);
        return drawn;
    }
}
