using System;

[Serializable]
public struct GameConfig
{
    public int InitialLives;
    public ShipConfig Ship;
    public float NextWaveCooldown;
    public int InitialAsteroids;
    public int ExtraAsteroidsPerWave;
    public FloatRange SpawnRange;
    public FloatRange AsteroidSpeedRange;
    public float AsteroidSplitAngle;
    public float BulletSpeed;
    public float BulletTravelDistance;
    public ScoringConfig Scoring;
    public float ReviveCooldown;
    public SaucersConfig Saucers;
}

[Serializable]
public struct FloatRange
{
    public float Min;
    public float Max;
    public float Length => Max - Min;
}

public enum AsteroidType
{
    Small,
    Medium,
    Large
}

public enum SaucerType
{
    Small,
    Large
}

[Serializable]
public struct AsteroidScoreConfig
{
    public AsteroidType Type;
    public int Score;
}

[Serializable]
public struct SaucerScoreConfig
{
    public SaucerType Type;
    public int Score;
}

[Serializable]
public struct ScoringConfig
{
    public AsteroidScoreConfig[] Asteroids;
    public SaucerScoreConfig[] Saucers;
}

[Serializable]
public struct SaucersConfig
{
    public SaucerConfig[] Saucers;
    public SaucerSpawnConfig[] SpawnConfigs;
    public float SpawnCooldown;
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
}

[Serializable]
public struct SaucerMovementConfig
{
    public float Speed;
    public float TurnChance;
    public float TurnCooldown;
}