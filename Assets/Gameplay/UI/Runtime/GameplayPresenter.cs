using System.Threading;
using System.Threading.Tasks;
using Gameplay.Simulation.Runtime;
using UnityEngine;

namespace Gameplay.UI.Runtime
{
    public class GameplayPresenter
    {
        readonly GameplayViewLibrary gameplayViewLibrary;
        readonly GameplayFactory gameplayFactory;

        public GameplayPresenter(GameplayViewLibrary gameplayViewLibrary, GameplayFactory gameplayFactory)
        {
            this.gameplayViewLibrary = gameplayViewLibrary;
            this.gameplayFactory = gameplayFactory;
        }

        public async Task PresentAsync(CancellationToken ct)
        {
            var cameras = GameObject.Instantiate<CameraGroup>(gameplayViewLibrary.Cameras);
            var gameplayView = GameObject.Instantiate<GameplayView>(gameplayViewLibrary.GameplayViewPrefab);
            var inputProvider = new KeyboardInputProvider();

            using GameplayBootstrap gameplay = gameplayFactory.Create(inputProvider, cameras);

            await gameplay.WaitForCompletionAsync(ct);

            GameObject.Destroy(gameplayView.gameObject);

            var gameOverView = GameObject.Instantiate<GameOverView>(gameplayViewLibrary.GameOverViewPrefab);
            await gameOverView.WaitForCompletionAsync(ct);

            GameObject.Destroy(gameOverView.gameObject);
            GameObject.Destroy(cameras.gameObject);
        }
    }
}