using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    public class GameplayFactory
    {
        readonly GameplayAssets assets;
        readonly GameConfig gameConfig;

        public GameplayFactory(GameplayAssets assets, GameConfig gameConfig)
        {
            this.assets = assets;
            this.gameConfig = gameConfig;
        }

        public GameplayBootstrap Create()
        {
            var playerShip = GameObject.Instantiate<Ship>(assets.ShipPrefab);
            playerShip.Setup(gameConfig.Ship);
            playerShip.Position = new Vector2(0, 0);
            playerShip.IsDestroyed = false;
            playerShip.Enable();

            var gameState = new GameState
            {
                Wave = 0,
                Player = new PlayerState
                {
                    Score = 0,
                    Lives = gameConfig.InitialLives,
                },
                PlayerShip = playerShip,
                Asteroids = new HashSet<Asteroid>(),
                Saucers = new List<Saucer>(),
                Bullets = new List<Bullet>(),
                SaucerSpawnCooldown = gameConfig.Saucers.SpawnCooldown,
            };

            var asteroidsPool = new ObjectPool<Asteroid>(assets.AsteroidPrefab);
            var bulletsPool = new ObjectPool<Bullet>(assets.BulletPrefab);
            var saucersPool = new ObjectPool<Saucer>(assets.SaucerPrefab);

        var cameras = GameObject.Instantiate<CameraGroup>(assets.Cameras);

        var shipController = new ShipController();

        var gameLoopObject = new GameObject("GameLoop");
        var bootstrap = gameLoopObject.AddComponent<GameplayBootstrap>();
        bootstrap.Setup(gameState, gameConfig, cameras, asteroidsPool, bulletsPool, saucersPool, shipController);

            return bootstrap;
        }
    }
}