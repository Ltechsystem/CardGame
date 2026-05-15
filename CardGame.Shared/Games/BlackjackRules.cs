using CardGame.Enums;
using CardGame.Models;

namespace CardGame.Games;

public static class BlackjackRules
{
    public static int HandValue(IEnumerable<Card> hand)
    {
        int total = 0, aces = 0;
        foreach (var card in hand)
        {
            total += card.Value;
            if (card.Rank == PlayingCardNo.Ace) aces++;
        }
        while (total > 21 && aces-- > 0) total -= 10;
        return total;
    }

    public static bool IsBust(IEnumerable<Card> hand) => HandValue(hand) > 21;

    // Two cards totalling 21
    public static bool IsNatural(IList<Card> hand) =>
        hand.Count == 2 && HandValue(hand) == 21;
}
