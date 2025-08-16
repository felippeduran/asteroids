using System;
using System.Linq;

[Serializable]
public struct AsteroidsConfig
{
    public int Initial;
    public int ExtraPerWave;
    public float NextWaveCooldown;
    public FloatRange SpawnRange;
    public FloatRange SpeedRange;
    public float SplitAngle;
    public AsteroidScoreConfig[] Scores;
}

public enum AsteroidType
{
    Small,
    Medium,
    Large
}

[Serializable]
public struct AsteroidScoreConfig
{
    public AsteroidType Type;
    public int Score;
}

public static class AsteroidsConfigExtensions
{
    public static AsteroidScoreConfig GetScoreConfigFor(this AsteroidScoreConfig[] configs, AsteroidType type)
    {
        return configs.First(c => c.Type == type);
    }

    public static int GetScoreFor(this AsteroidScoreConfig[] configs, AsteroidType type)
    {
        return configs.GetScoreConfigFor(type).Score;
    }
}