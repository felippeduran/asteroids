using System;

namespace Gameplay.Simulation.Runtime
{
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
}