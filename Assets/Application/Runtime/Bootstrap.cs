using UnityEngine;
using Gameplay.Simulation.Runtime;
using Gameplay.UI.Runtime;
using Metagame.UI.Runtime;
using UnityApplication = UnityEngine.Application;

namespace Application.Runtime
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] GameConfig gameConfig;
        [SerializeField] GameplayAssets gameplayAssets;
        [SerializeField] GameplayViewLibrary gameplayViewLibrary;
        [SerializeField] MetagameViewLibrary metagameViewLibrary;

        async void Start()
        {
            var exitToken = UnityApplication.exitCancellationToken;
            while (!exitToken.IsCancellationRequested)
            {
                var mainMenuPresenter = new MainMenuPresenter(metagameViewLibrary);
                await mainMenuPresenter.PresentAsync(exitToken);

                var gameplayFactory = new GameplayFactory(gameplayAssets, gameConfig);

                await new GameplayPresenter(gameplayViewLibrary, gameplayFactory).PresentAsync(exitToken);
            }
        }
    }
}