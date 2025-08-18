using System;

namespace Gameplay.Simulation.Runtime
{
    [Serializable]
    public struct LivesConfig
    {
        public int InitialLives;
        public int ScoreForExtraLife;
    }
}