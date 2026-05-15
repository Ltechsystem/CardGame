using CardGame.Enums;

namespace CardGame.Models;

public class Card
{
    public PlayingCardNo Rank { get; }
    public PlayingCardSuite Suite { get; }

    public Card(PlayingCardNo rank, PlayingCardSuite suite)
    {
        Rank = rank;
        Suite = suite;
    }

    public int Value => Rank switch
    {
        PlayingCardNo.Ace   => 11,
        PlayingCardNo.Jack  => 10,
        PlayingCardNo.Queen => 10,
        PlayingCardNo.King  => 10,
        _                   => (int)Rank
    };

    public string RankSymbol => Rank switch
    {
        PlayingCardNo.Ace   => "A",
        PlayingCardNo.Jack  => "J",
        PlayingCardNo.Queen => "Q",
        PlayingCardNo.King  => "K",
        _                   => ((int)Rank).ToString()
    };

    public string SuiteSymbol => Suite switch
    {
        PlayingCardSuite.Hearts   => "♥",
        PlayingCardSuite.Diamonds => "♦",
        PlayingCardSuite.Clubs    => "♣",
        PlayingCardSuite.Spades   => "♠",
        _                         => "?"
    };

    public bool IsRed => Suite is PlayingCardSuite.Hearts or PlayingCardSuite.Diamonds;

    public override string ToString() => $"{Rank} of {Suite}";
}
