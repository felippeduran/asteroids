using System;
using System.Collections.Generic;

namespace Gameplay.Simulation.Runtime
{
    [Serializable]
    public struct GameState
    {
        public AsteroidWaveState WaveState;
        public SaucersState SaucersState;
        public PlayerState PlayerState;
        public Ship PlayerShip;
        public HashSet<IAsteroid> Asteroids;
        public List<ISaucer> Saucers;
        public List<IBullet> Bullets;
    }
}