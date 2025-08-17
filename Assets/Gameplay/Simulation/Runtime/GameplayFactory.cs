using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Company.Utilities.Runtime;

namespace Gameplay.Simulation.Runtime
{
    public class GameplayFactory
    {
        readonly GameplayAssetLibrary assetLibrary;
        readonly GameConfig gameConfig;

        public GameplayFactory(GameplayAssetLibrary assetLibrary, GameConfig gameConfig)
        {
            this.assetLibrary = assetLibrary;
            this.gameConfig = gameConfig;
        }

        public async Task<IGameplay> CreateAsync(IInputProvider inputProvider, ICameraGroup cameraGroup)
        {
            var assets = await assetLibrary.LoadAllAsync();

            var playerShip = GameObject.Instantiate(assets.Ship);

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

            var asteroidsPool = new ObjectPool<Asteroid>(assets.Asteroid);
            var bulletsPool = new ObjectPool<Bullet>(assets.Bullet);
            var saucersPool = new ObjectPool<Saucer>(assets.Saucer);

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

            var disposer = new BatchDisposer(
                asteroidsPool,
                bulletsPool,
                saucersPool,
                new DisposableGameObject(playerShip.gameObject),
                new DisposableGameObject(gameLoopObject),
                assets
            );

            bootstrap.Setup(gameState, gameConfig, gameSystems, disposer);

            return bootstrap;
        }
    }
}