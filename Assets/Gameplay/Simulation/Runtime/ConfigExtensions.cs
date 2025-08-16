using System.Linq;

public static class ConfigExtensions
{
    public static AsteroidScoreConfig GetScoreConfigFor(this AsteroidScoreConfig[] configs, AsteroidType type)
    {
        return configs.First(c => c.Type == type);
    }

    public static int GetScoreFor(this AsteroidScoreConfig[] configs, AsteroidType type)
    {
        return configs.GetScoreConfigFor(type).Score;
    }

    public static int GetScoreFor(this SaucerScoreConfig[] configs, SaucerType type)
    {
        return configs.First(c => c.Type == type).Score;
    }

    public static SaucerConfig GetSaucerConfigFor(this SaucerConfig[] configs, SaucerType type)
    {
        return configs.First(c => c.Type == type);
    }
}