using CardGame.Models;

namespace CardGame.Games;

public interface IBlackjackGame : ICardGame
{
    Player HumanPlayer { get; }
    AiPlayer Dealer { get; }

    int ChipBalance { get; }
    int CurrentBet { get; }

    bool PlaceBet(int amount);   // returns false if insufficient chips
    void PlayerHit();
    void PlayerStand();
    int SettleBet();             // applies payout, returns net chip change; call once IsGameOver
}
