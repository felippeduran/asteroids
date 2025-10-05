using System.Numerics;
using Company.Utilities.Runtime;

namespace Gameplay.Simulation.Runtime
{
    public interface IBullet : IPoolable
    {
        Vector2 Position { get; set; }
        Vector2 LinearVelocity { get; set; }
        bool IsDestroyed { get; set; }
        bool IsPlayerBullet { get; set; }
        int Score { get; set; }
        float TotalTraveledDistance { get; set; }
    }
}
