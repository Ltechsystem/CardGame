using CardGame.Data;
using CardGame.Entities;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Services;

public class BalanceService
{
    private const int DefaultBalance = 1_000;

    public async Task<int> GetBalanceAsync(string playerName)
    {
        await using var db = new CardGameDbContext();
        await EnsureTableAsync(db);
        var entry = await db.PlayerBalances.FirstOrDefaultAsync(p => p.PlayerName == playerName);
        return entry?.Chips ?? DefaultBalance;
    }

    public async Task SetBalanceAsync(string playerName, int chips)
    {
        await using var db = new CardGameDbContext();
        await EnsureTableAsync(db);
        var entry = await db.PlayerBalances.FirstOrDefaultAsync(p => p.PlayerName == playerName);
        if (entry is null)
            db.PlayerBalances.Add(new PlayerBalance { PlayerName = playerName, Chips = chips });
        else
            entry.Chips = chips;
        await db.SaveChangesAsync();
    }

    // Creates the table if the DB existed before PlayerBalances was added to the model.
    private static async Task EnsureTableAsync(CardGameDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS PlayerBalances (
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerName TEXT    NOT NULL,
                Chips      INTEGER NOT NULL
            )");
    }
}
