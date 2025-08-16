using System;
using System.Collections.Generic;

[Serializable]
public struct GameState
{
    public int Wave;
    public bool NextWave;
    public float NextWaveCooldown;
    public float SaucerSpawnCooldown;
    public PlayerState Player;
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