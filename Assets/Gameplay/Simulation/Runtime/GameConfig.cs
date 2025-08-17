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
}

[Serializable]
public struct FloatRange
{
    public float Min;
    public float Max;
    public float Length => Max - Min;
}
