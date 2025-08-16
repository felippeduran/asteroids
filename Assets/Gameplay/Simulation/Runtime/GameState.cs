using System;
using System.Collections.Generic;

[Serializable]
public struct GameState
{
    public int Lives;
    public int Score;
    public int Wave;
    public bool NextWave;
    public float NextWaveCooldown;
    public bool Reviving;
    public float ReviveCooldown;
    public float SaucerSpawnCooldown;
    public Ship PlayerShip;
    public HashSet<Asteroid> Asteroids;
    public List<Saucer> Saucers;
    public List<Bullet> Bullets;

    public readonly bool GameOver => Lives <= 0;
}