using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Company.Utilities.Runtime;

namespace Gameplay.Simulation.Runtime
{
    public interface IGameplayAssetLibrary
    {
        Task<IGameplayAssets> LoadAllAsync();
    }

    public interface IPhysicsFactory
    {
        INearbyPlayerFinder CreateNearbyPlayerFinder();
    }

    public interface IGameplayAssets
    {
        public IObjectPool<IShip> Ships { get; init; }
        public IObjectPool<IAsteroid> Asteroids { get; init; }
        public IObjectPool<IBullet> Bullets { get; init; }
        public IObjectPool<ISaucer> Saucers { get; init; }

        IGameplay CreateGameplay(GameState gameState, GameConfig gameConfig, GameSystems gameSystems);
    }

    public interface IGameplayFactory
    {
        Task<IGameplay> CreateAsync(IInputProvider inputProvider, ICameraGroup cameraGroup);
    }

    public class GameplayFactory : IGameplayFactory
    {
        readonly IGameplayAssetLibrary assetLibrary;
        readonly IPhysicsFactory physicsFactory;
        readonly GameConfig gameConfig;

        public GameplayFactory(IGameplayAssetLibrary assetLibrary, IPhysicsFactory physicsFactory, GameConfig gameConfig)
        {
            this.assetLibrary = assetLibrary;
            this.physicsFactory = physicsFactory;
            this.gameConfig = gameConfig;
        }

        public async Task<IGameplay> CreateAsync(IInputProvider inputProvider, ICameraGroup cameraGroup)
        {
            var assets = await assetLibrary.LoadAllAsync();

            var playerShip = assets.Ships.Get();

            playerShip.Position = new System.Numerics.Vector2(0, 0);
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

            var gameSystems = new GameSystems
            {
                ShipSystem = new ShipSystem(inputProvider),
                BulletsSystem = new BulletsSystem(assets.Bullets),
                SaucersSystem = new SaucersSystem(physicsFactory.CreateNearbyPlayerFinder(), assets.Saucers),
                AsteroidsSystem = new AsteroidsSystem(assets.Asteroids),
                WorldLoopSystem = new WorldLoopSystem(),
                ExtraLifeSystem = new ExtraLifeSystem()
            };

            cameraGroup.SetWorldSize(gameConfig.WorldSize);

            return assets.CreateGameplay(gameState, gameConfig, gameSystems);
        }
    }
}