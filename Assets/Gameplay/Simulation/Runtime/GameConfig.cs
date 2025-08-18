using System;
using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    [Serializable]
    public struct GameConfig
    {
        public Vector2 WorldSize;
        public LivesConfig Lives;
        public ShipConfig Ship;
        public AsteroidsConfig Asteroids;
        public BulletsConfig Bullets;
        public SaucersConfig Saucers;
        public readonly Bounds WorldBounds => new Bounds(Vector2.zero, WorldSize);
    }

    [Serializable]
    public struct FloatRange
    {
        public float Min;
        public float Max;
        public float Length => Max - Min;
    }
}
