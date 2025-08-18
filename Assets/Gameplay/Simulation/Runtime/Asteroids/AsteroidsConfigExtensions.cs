using System;
using System.Linq;

namespace Gameplay.Simulation.Runtime
{
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
}