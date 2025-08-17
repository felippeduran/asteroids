using System;
using UnityEngine;

[Serializable]
public struct GameConfig
{
    public int InitialLives;
    public Vector2 WorldSize;
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
