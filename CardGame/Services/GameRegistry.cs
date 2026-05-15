using System.Reflection;
using CardGame.Enums;
using CardGame.Games;

namespace CardGame.Services;

public class GameRegistry
{
    private readonly List<ICardGamePlugin> _plugins = [];

    public IReadOnlyList<ICardGamePlugin> Plugins => _plugins.AsReadOnly();

    public void Register(ICardGamePlugin plugin) => _plugins.Add(plugin);

    public ICardGame? CreateGame(GameType gameType, string playerName, Difficulty difficulty, int startingChips = 0)
        => _plugins.FirstOrDefault(p => p.GameType == gameType)?.CreateGame(playerName, difficulty, startingChips);

    // Scans the app's output directory for CardGame.*.dll files and loads any ICardGamePlugin they contain.
    // Dropping a new game DLL next to the exe is all that's needed to add it.
    public void LoadPlugins()
    {
        var baseDir = AppContext.BaseDirectory;
        foreach (var dll in Directory.GetFiles(baseDir, "CardGame.*.dll"))
        {
            var filename = Path.GetFileNameWithoutExtension(dll);
            if (filename is "CardGame.Shared") continue;

            try
            {
                var assembly = Assembly.LoadFrom(dll);
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(ICardGamePlugin).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract)
                        continue;

                    var plugin = (ICardGamePlugin)Activator.CreateInstance(type)!;
                    Register(plugin);
                }
            }
            catch
            {
                // Skip assemblies that can't be loaded (wrong TFM, missing deps, etc.)
            }
        }
    }
}
