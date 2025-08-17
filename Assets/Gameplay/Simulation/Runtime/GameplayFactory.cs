using System;
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

        public GameplayBootstrap Create(IInputProvider inputProvider, ICameraGroup cameraGroup)
        {
            var playerShip = GameObject.Instantiate<Ship>(assets.ShipPrefab);
            playerShip.Position = new Vector2(0, 0);
            playerShip.IsTeamPlayer = true;
            playerShip.IsDestroyed = false;
            playerShip.Enable();

            var gameState = new GameState
            {
                WaveState = new AsteroidWaveState { Current = 0 },
                PlayerState = new PlayerState
                {
                    Score = 0,
                    Lives = gameConfig.InitialLives,
                },
                SaucersState = new SaucersState { SpawnCooldown = gameConfig.Saucers.SpawnCooldown },
                PlayerShip = playerShip,
                Asteroids = new HashSet<Asteroid>(),
                Saucers = new List<Saucer>(),
                Bullets = new List<Bullet>(),
            };

            var asteroidsPool = new ObjectPool<Asteroid>(assets.AsteroidPrefab);
            var bulletsPool = new ObjectPool<Bullet>(assets.BulletPrefab);
            var saucersPool = new ObjectPool<Saucer>(assets.SaucerPrefab);

            var gameSystems = new GameSystems
            {
                ShipController = new ShipController(inputProvider),
                BulletsController = new BulletsController(bulletsPool),
                SaucersController = new SaucersController(saucersPool),
                AsteroidsController = new AsteroidsController(asteroidsPool),
                WorldLoopController = new WorldLoopController()
            };

            cameraGroup.SetWorldSize(gameConfig.WorldSize);

            var gameLoopObject = new GameObject("GameLoop");
            var bootstrap = gameLoopObject.AddComponent<GameplayBootstrap>();

            var gameplayDisposer = new GameplayDisposer(playerShip, gameLoopObject, new IDisposable[] { asteroidsPool, bulletsPool, saucersPool });

            bootstrap.Setup(gameState, gameConfig, gameSystems, gameplayDisposer);

            return bootstrap;
        }
    }
}