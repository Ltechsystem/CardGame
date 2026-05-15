using CardGame.Entities;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Data;

public class CardGameDbContext : DbContext
{
    public DbSet<HighscoreEntry> Highscores => Set<HighscoreEntry>();
    public DbSet<PlayerBalance> PlayerBalances => Set<PlayerBalance>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbPath = Path.Combine(folder, "CardGame", "cardgame.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        options.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HighscoreEntry>(e =>
        {
            e.HasKey(h => h.Id);
            e.Property(h => h.PlayerName).IsRequired().HasMaxLength(50);
            e.Property(h => h.Game).HasConversion<int>();
            e.Property(h => h.Difficulty).HasConversion<int>();
        });

        modelBuilder.Entity<PlayerBalance>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.PlayerName).IsRequired().HasMaxLength(50);
        });
    }
}
