using System;
using System.Linq;

namespace Gameplay.Simulation.Runtime
{
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
}