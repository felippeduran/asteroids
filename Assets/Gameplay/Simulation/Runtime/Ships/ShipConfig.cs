using System;

namespace Gameplay.Simulation.Runtime
{
    [Serializable]
    public struct ShipConfig
    {
        public float ThrustForce;
        public float TurnSpeed;
        public float ReviveCooldown;
        public int MaxAmmo;
        public float AmmoReloadRate;
        public float FireRate;
        public float TeleportTime;
    }
}

