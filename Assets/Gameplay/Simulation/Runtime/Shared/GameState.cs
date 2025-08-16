using System;
using System.Collections.Generic;

[Serializable]
public struct GameState
{
    public AsteroidWaveState WaveState;
    public SaucersState SaucersState;
    public PlayerState PlayerState;
    public Ship PlayerShip;
    public HashSet<Asteroid> Asteroids;
    public List<Saucer> Saucers;
    public List<Bullet> Bullets;
}

[Serializable]
public struct PlayerState
{
    public int Score;
    public int Lives;
    public bool Reviving;
    public float ReviveCooldown;
    public readonly bool GameOver => Lives <= 0;
}