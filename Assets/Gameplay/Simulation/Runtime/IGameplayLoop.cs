using System;
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

    public interface IGameplayLoop : IGameplay
    {
        void Setup(GameState gameState, GameConfig gameConfig, GameSystems gameSystems, IDisposable disposer);
    }
}