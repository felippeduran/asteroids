using System;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Gameplay.Simulation.Runtime
{
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
            var bulletsFired = gameSystems.ShipSystem.UpdateShip(deltaTime, gameState.PlayerShip, ref gameState.PlayerState, gameConfig.Ship, gameConfig.WorldBounds);
            var moreBulletsFired = gameSystems.SaucersSystem.UpdateSaucers(deltaTime, ref gameState.SaucersState, gameState.Saucers, gameConfig.Saucers, gameConfig.WorldBounds);
            bulletsFired = bulletsFired.Concat(moreBulletsFired).ToArray();
            gameSystems.BulletsSystem.UpdateBullets(deltaTime, bulletsFired, ref gameState.PlayerState, gameState.Bullets, gameConfig.Bullets);

            gameSystems.ExtraLifeSystem.UpdateExtraLives(ref gameState.PlayerState, gameConfig.Lives);

            gameSystems.AsteroidsSystem.UpdateAsteroids(deltaTime, ref gameState.WaveState, gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameConfig.Asteroids, gameConfig.WorldBounds);
            gameSystems.WorldLoopSystem.LoopObjectsThroughWorld(gameState.PlayerShip, gameState.Asteroids, gameState.Saucers, gameState.Bullets, gameConfig.WorldBounds);


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