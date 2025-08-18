using UnityEngine;
using Company.Utilities.Runtime;

namespace Gameplay.Simulation.Runtime
{
    public interface IAsteroid : IPoolable
    {
        AsteroidType Type { get; set; }
        int ScoreWorth { get; set; }
        Vector2 Position { get; set; }
        Vector2 LinearVelocity { get; set; }
        bool IsDestroyed { get; set; }
    }
}
