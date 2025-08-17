using System.Threading;
using System.Threading.Tasks;
using Gameplay.Simulation.Runtime;
using UnityEngine;

namespace Gameplay.UI.Runtime
{
    public interface IGameOverView
    {
        Task WaitForCompletionAsync(CancellationToken ct);
        void Setup(int score);
    }

    public interface IGameplayView
    {
        void UpdateUI(int lives, int score);
    }

    public class GameplayPresenter
    {
        readonly GameplayUIAssetLibrary assetLibrary;
        readonly GameplayFactory gameplayFactory;

        public GameplayPresenter(GameplayUIAssetLibrary assetLibrary, GameplayFactory gameplayFactory)
        {
            this.assetLibrary = assetLibrary;
            this.gameplayFactory = gameplayFactory;
        }

        public async Task PresentAsync(CancellationToken ct)
        {
            using var camerasHandle = await assetLibrary.CreateCameraGroupAsync();
            var inputProvider = new KeyboardInputProvider();
            using IGameplay gameplay = await gameplayFactory.CreateAsync(inputProvider, camerasHandle.Obj);

            var gameOver = false;
            using (var gameplayView = await assetLibrary.CreateGameplayViewAsync())
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