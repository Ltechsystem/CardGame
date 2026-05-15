using CardGame.Config;
using CardGame.Data;
using CardGame.Entities;
using CardGame.Enums;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Services;

public class HighscoreService
{
    public async Task SaveScoreAsync(string playerName, GameType game, Difficulty difficulty, int baseScore)
    {
        var finalScore = (int)Math.Round(baseScore * DifficultySettings.GetMultiplier(difficulty));

        await using var db = new CardGameDbContext();
        await db.Database.EnsureCreatedAsync();

        db.Highscores.Add(new HighscoreEntry
        {
            PlayerName = playerName,
            Game = game,
            Difficulty = difficulty,
            FinalScore = finalScore,
            Date = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task<List<HighscoreEntry>> GetTopScoresAsync(
        GameType? game = null,
        Difficulty? difficulty = null,
        int count = 10)
    {
        await using var db = new CardGameDbContext();
        await db.Database.EnsureCreatedAsync();

        var query = db.Highscores.AsQueryable();

        if (game.HasValue)
            query = query.Where(h => h.Game == game.Value);

        if (difficulty.HasValue)
            query = query.Where(h => h.Difficulty == difficulty.Value);

        return await query
            .OrderByDescending(h => h.FinalScore)
            .ThenBy(h => h.Date)
            .Take(count)
            .ToListAsync();
    }
}
