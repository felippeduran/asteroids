using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using Gameplay.Simulation.Runtime;
using Company.Utilities.Runtime.Unity;

namespace Gameplay.Simulation.Runtime.Unity
{
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
            Destroy(gameObject);
            disposer.Dispose();
        }

        public void UpdateSimulation(float deltaTime)
        {
            gameSystems.Update(deltaTime, ref gameState, gameConfig);

            OnUpdate(gameState);
        }

        void OnDrawGizmos()
        {
            if (gameState.PlayerShip != null)
            {
                Vector3 position = gameState.PlayerShip.Position.To3D().ToUnity();
                Gizmos.DrawWireSphere(position, gameConfig.Asteroids.SpawnRange.Min);
                Gizmos.DrawWireSphere(position, gameConfig.Asteroids.SpawnRange.Max);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, ((System.Numerics.Vector2)gameConfig.WorldSize).To3D().ToUnity());
        }
    }
}