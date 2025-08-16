using System;
using System.Linq;

[Serializable]
public struct SaucersConfig
{
    public SaucerConfig[] Saucers;
    public SaucerSpawnConfig[] SpawnConfigs;
    public SaucerScoreConfig[] Scores;
    public float SpawnCooldown;
}

public enum SaucerType
{
    Small,
    Large
}

[Serializable]
public struct SaucerScoreConfig
{
    public SaucerType Type;
    public int Score;
}

[Serializable]
public struct SaucerSpawnConfig
{
    public SaucerType Type;
    public float Chance;
}

[Serializable]
public struct SaucerConfig
{
    public SaucerType Type;
    public SaucerMovementConfig Movement;
    public SaucerAimConfig Aim;
}

[Serializable]
public struct SaucerAimConfig
{
    public float FireRate;
    public float FireAngle;
    public float SightDistance;
}

[Serializable]
public struct SaucerMovementConfig
{
    public float Speed;
    public float TurnChance;
    public float TurnCooldown;
}

public static class SaucersConfigExtensions
{
    public static int GetScoreFor(this SaucerScoreConfig[] configs, SaucerType type)
    {
        return configs.First(c => c.Type == type).Score;
    }

    public static SaucerConfig GetSaucerConfigFor(this SaucerConfig[] configs, SaucerType type)
    {
        return configs.First(c => c.Type == type);
    }
}