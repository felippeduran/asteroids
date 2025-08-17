using System.Threading;
using System.Threading.Tasks;
using Gameplay.Simulation.Runtime;

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
            using var cameras = gameplayViewLibrary.CreateCameraGroup();
            var inputProvider = new KeyboardInputProvider();
            using GameplayBootstrap gameplay = gameplayFactory.Create(inputProvider, cameras);

            using (var gameplayView = gameplayViewLibrary.CreateGameplayView())
            {
                await gameplay.WaitForCompletionAsync(ct);
            }

            using var gameOverView = gameplayViewLibrary.CreateGameOverView();
            await gameOverView.WaitForCompletionAsync(ct);
        }
    }
}