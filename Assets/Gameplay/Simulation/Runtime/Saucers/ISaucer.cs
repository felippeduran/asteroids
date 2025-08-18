using UnityEngine;
using Company.Utilities.Runtime;

namespace Gameplay.Simulation.Runtime
{
    public interface ISaucer : IPoolable
    {
        SaucerType Type { get; set; }
        Vector2 Position { get; set; }
        bool IsDestroyed { get; set; }
        Vector2 LinearVelocity { get; set; }
        bool IsTeamPlayer { get; set; }
        float TurnCooldown { get; set; }
        float ShootCooldown { get; set; }
        Saucer.ShootRingData ShootRing { get; }
    }
}
