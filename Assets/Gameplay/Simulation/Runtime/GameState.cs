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
    public int Score { get; set; }
    public int Lives { get; set; }
    public bool Reviving { get; set; }
    public float ReviveCooldown { get; set; }
    public readonly bool GameOver => Lives <= 0;
}