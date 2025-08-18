using System;
using UnityEngine;
using Gameplay.Simulation.Runtime;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Gameplay.Simulation.Runtime
{
    public struct GameSystems
    {
        public ShipController ShipController;
        public BulletsController BulletsController;
        public SaucersController SaucersController;
        public AsteroidsController AsteroidsController;
        public WorldLoopController WorldLoopController;
        public ExtraLifeController ExtraLifeController;
    }

    public interface IGameplay : IDisposable
    {
        Task<bool> WaitForCompletionAsync(CancellationToken ct);
        void UpdateSimulation(float deltaTime);
        GameState GameState { get; }
    }

    public class GameplayLoop : MonoBehaviour, IGameplay
    {
        public event Action<GameState> OnUpdate = delegate { };

        [SerializeField] GameConfig gameConfig;
        [SerializeField] GameState gameState;

        GameSystems gameSystems;
        IDisposable disposer;

        public GameState GameState => gameState;

        public void Setup(GameState gameState, GameConfig gameConfig, GameSystems gameSystems, IDisposable disposer)
        {
            this.gameState = gameState;
            this.gameConfig = gameConfig;
            this.gameSystems = gameSystems;
            this.disposer = disposer;
        }

        public async Task<bool> WaitForCompletionAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && !gameState.PlayerState.GameOver)
            {
                await Task.Yield();
            }

            return gameState.PlayerState.GameOver;
        }

        public void Dispose()
        {
            disposer.Dispose();
        }

        public void UpdateSimulation(float deltaTime)
        {
            var bulletsFired = gameSystems.ShipController.UpdateShip(deltaTime, gameState.PlayerShip, ref gameState.PlayerState, gameConfig.Ship, gameConfig.WorldBounds);
            var moreBulletsFired = gameSystems.SaucersController.UpdateSaucers(deltaTime, ref gameState.SaucersState, gameState.Saucers, gameConfig.Saucers, gameConfig.WorldBounds);
            bulletsFired = bulletsFired.Concat(moreBulletsFired).ToArray();
            gameSystems.BulletsController.UpdateBullets(deltaTime, bulletsFired, ref gameState.PlayerState, gameState.Bullets, gameConfig.Bullets);

            gameSystems.ExtraLifeController.UpdateExtraLives(ref gameState.PlayerState, gameConfig.Lives);

            gameSystems.AsteroidsController.UpdateAsteroids(deltaTime, ref gameState.WaveState, gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameConfig.Asteroids, gameConfig.WorldBounds);
            gameSystems.WorldLoopController.LoopObjectsThroughWorld(gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameState.Bullets, gameConfig.WorldBounds);


            OnUpdate(gameState);
        }

        void OnDrawGizmos()
        {
            if (gameState.PlayerShip != null)
            {
                Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.Asteroids.SpawnRange.Min);
                Gizmos.DrawWireSphere(gameState.PlayerShip.Position, gameConfig.Asteroids.SpawnRange.Max);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, gameConfig.WorldSize);
        }
    }
}