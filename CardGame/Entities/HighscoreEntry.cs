using CardGame.Enums;

namespace CardGame.Entities;

public class HighscoreEntry
{
    public int Id { get; set; }
    public required string PlayerName { get; set; }
    public GameType Game { get; set; }
    public Difficulty Difficulty { get; set; }
    public int FinalScore { get; set; }
    public DateTime Date { get; set; }
}
