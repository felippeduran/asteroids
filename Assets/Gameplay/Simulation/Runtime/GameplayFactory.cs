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
            playerShip.Ammo = gameConfig.Ship.MaxAmmo;
            playerShip.Enable();

            var gameState = new GameState
            {
                WaveState = new AsteroidWaveState { Current = 0 },
                PlayerState = new PlayerState
                {
                    Score = 0,
                    Lives = gameConfig.Lives.InitialLives,
                    NextLifeScore = gameConfig.Lives.ScoreForExtraLife,
                },
                SaucersState = new SaucersState { SpawnCooldown = gameConfig.Saucers.SpawnCooldown },
                PlayerShip = playerShip,
                Asteroids = new HashSet<IAsteroid>(),
                Saucers = new List<ISaucer>(),
                Bullets = new List<IBullet>(),
            };

            var asteroidsPool = new ObjectPool<IAsteroid, Asteroid>(assets.Asteroid);
            var bulletsPool = new ObjectPool<IBullet, Bullet>(assets.Bullet);
            var saucersPool = new ObjectPool<ISaucer, Saucer>(assets.Saucer);

            var gameSystems = new GameSystems
            {
                ShipSystem = new ShipSystem(inputProvider),
                BulletsSystem = new BulletsSystem(bulletsPool),
                SaucersSystem = new SaucersSystem(saucersPool),
                AsteroidsSystem = new AsteroidsSystem(asteroidsPool),
                WorldLoopSystem = new WorldLoopSystem(),
                ExtraLifeSystem = new ExtraLifeSystem()
            };

            cameraGroup.SetWorldSize(gameConfig.WorldSize);

            var gameLoopObject = new GameObject("GameLoop");
            var bootstrap = gameLoopObject.AddComponent<GameplayLoop>();

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