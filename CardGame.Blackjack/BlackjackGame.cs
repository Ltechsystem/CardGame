using CardGame.Enums;
using CardGame.Games;
using CardGame.Models;

namespace CardGame.Blackjack;

public class BlackjackGame : IBlackjackGame
{
    public Player HumanPlayer { get; }
    public AiPlayer Dealer { get; }
    private readonly Deck _deck = new();

    public GameType GameType  => GameType.Blackjack;
    public Difficulty Difficulty => Dealer.Difficulty;
    public bool IsGameOver { get; private set; }

    public int ChipBalance { get; private set; }
    public int CurrentBet  { get; private set; }

    // Dealer stops drawing at or above this threshold — adjusts per difficulty.
    private int DealerStopAt => Dealer.Difficulty switch
    {
        Difficulty.Easy   => 14, // draws more → more likely to bust → easier
        Difficulty.Medium => 17, // standard casino rule
        Difficulty.Hard   => 18, // more conservative → harder to beat
        _                 => 17
    };

    public BlackjackGame(string playerName, Difficulty difficulty, int startingChips)
    {
        HumanPlayer  = new Player(playerName);
        Dealer       = new AiPlayer("Dealer", difficulty);
        ChipBalance  = startingChips;
    }

    public bool PlaceBet(int amount)
    {
        if (amount <= 0 || amount > ChipBalance) return false;
        CurrentBet = amount;
        ChipBalance -= amount;
        return true;
    }

    public void StartGame()
    {
        _deck.Reset();
        _deck.Shuffle();
        HumanPlayer.ClearHand();
        Dealer.ClearHand();
        HumanPlayer.AddToHand(_deck.Draw(2));
        Dealer.AddToHand(_deck.Draw(2));
        IsGameOver = false;

        // Auto-resolve a natural blackjack immediately.
        if (BlackjackRules.IsNatural(HumanPlayer.Hand) ||
            BlackjackRules.IsNatural(Dealer.Hand))
            IsGameOver = true;
    }

    public void PlayerHit()
    {
        var card = _deck.Draw();
        if (card is not null) HumanPlayer.AddToHand(card);
        if (BlackjackRules.IsBust(HumanPlayer.Hand))
            IsGameOver = true;
    }

    public void PlayerStand()
    {
        while (BlackjackRules.HandValue(Dealer.Hand) < DealerStopAt && !_deck.IsEmpty)
            Dealer.AddToHand(_deck.Draw()!);
        IsGameOver = true;
    }

    // Returns net chip change (positive = win, negative = loss, zero = push).
    public int SettleBet()
    {
        int pv = BlackjackRules.HandValue(HumanPlayer.Hand);
        int dv = BlackjackRules.HandValue(Dealer.Hand);
        bool playerNatural = BlackjackRules.IsNatural(HumanPlayer.Hand);
        bool dealerNatural = BlackjackRules.IsNatural(Dealer.Hand);

        int payout;
        if (pv > 21)
        {
            payout = 0; // bust — bet already deducted
        }
        else if (playerNatural && !dealerNatural)
        {
            // Blackjack pays 3:2
            payout = CurrentBet + (int)Math.Ceiling(CurrentBet * 1.5);
        }
        else if (dealerNatural && !playerNatural)
        {
            payout = 0;
        }
        else if (dv > 21 || pv > dv)
        {
            payout = CurrentBet * 2;
        }
        else if (pv == dv)
        {
            payout = CurrentBet; // push
        }
        else
        {
            payout = 0;
        }

        ChipBalance += payout;
        int net = payout - CurrentBet;
        CurrentBet = 0;
        return net;
    }

    public int GetBaseScore() => ChipBalance;

    public Player? GetWinner()
    {
        int pv = BlackjackRules.HandValue(HumanPlayer.Hand);
        int dv = BlackjackRules.HandValue(Dealer.Hand);
        if (pv > 21) return Dealer;
        if (dv > 21 || pv > dv) return HumanPlayer;
        if (dv > pv) return Dealer;
        return null;
    }
}
