using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Gameplay.Simulation.Runtime;
using Company.Utilities.Runtime;

namespace Gameplay.Presentation.Runtime
{
    public interface IGameplayAssetLibrary
    {
        Task<IDisposableInstanceHandle<ICameraGroup>> CreateCameraGroupAsync();
        Task<IDisposableInstanceHandle<IGameplayView>> CreateGameplayViewAsync();
        Task<IDisposableInstanceHandle<IGameOverView>> CreateGameOverViewAsync();
    }

    public interface IGameOverView
    {
        Task WaitForCompletionAsync(CancellationToken ct);
        void Setup(int score);
    }

    public interface IGameplayView
    {
        InputData GetInput();
        void UpdateUI(int lives, int score);
    }

    public struct InputData
    {
        public bool TurnLeft;
        public bool TurnRight;
        public bool Thrust;
        public bool Fire;
        public bool Teleport;
    }

    public class GameplayPresenter
    {
        readonly IGameplayAssetLibrary assetLibrary;
        readonly GameplayFactory gameplayFactory;

        public GameplayPresenter(IGameplayAssetLibrary assetLibrary, GameplayFactory gameplayFactory)
        {
            this.assetLibrary = assetLibrary;
            this.gameplayFactory = gameplayFactory;
        }

        public async Task PresentAsync(CancellationToken ct)
        {
            using var camerasHandle = await assetLibrary.CreateCameraGroupAsync();

            var gameplayView = await assetLibrary.CreateGameplayViewAsync();

            var inputProvider = new UIInputProviderAdapter(gameplayView.Obj);
            using IGameplay gameplay = await gameplayFactory.CreateAsync(inputProvider, camerasHandle.Obj);

            var gameOver = false;
            using (gameplayView)
            {
                while (!ct.IsCancellationRequested && !gameplay.GameState.PlayerState.GameOver)
                {
                    gameplay.UpdateSimulation(Time.deltaTime);
                    gameplayView.Obj.UpdateUI(gameplay.GameState.PlayerState.Lives, gameplay.GameState.PlayerState.Score);
                    await Task.Yield();
                }

                gameOver = await gameplay.WaitForCompletionAsync(ct);
            }

            if (gameOver)
            {
                using var gameOverView = await assetLibrary.CreateGameOverViewAsync();
                gameOverView.Obj.Setup(gameplay.GameState.PlayerState.Score);
                await gameOverView.Obj.WaitForCompletionAsync(ct);
            }
        }
    }
}