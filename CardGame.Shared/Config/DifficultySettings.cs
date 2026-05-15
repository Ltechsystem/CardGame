using CardGame.Enums;

namespace CardGame.Config;

public static class DifficultySettings
{
    public static double GetMultiplier(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy   => 1.0,
        Difficulty.Medium => 1.5,
        Difficulty.Hard   => 2.0,
        _                 => 1.0
    };
}
