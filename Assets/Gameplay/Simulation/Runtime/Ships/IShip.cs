using System.Numerics;
using Company.Utilities.Runtime;

namespace Gameplay.Simulation.Runtime
{
    public interface IShip : IPoolable
    {
        Vector2 Position { get; set; }
        Vector2 Forward { get; set; }
        bool IsDestroyed { get; set; }
        bool IsTeamPlayer { get; set; }
        Vector2 ThrustForce { get; set; }
        float AngularVelocity { get; set; }
        bool IsThrusting { get; }
        Vector2 BulletSpawnPosition { get; }
        int Ammo { get; set; }
        float AmmoReloadCooldown { get; set; }
        float FireCooldown { get; set; }
        bool IsTeleporting { get; set; }
        float TeleportCooldown { get; set; }
    }
}