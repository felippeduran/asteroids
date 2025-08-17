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