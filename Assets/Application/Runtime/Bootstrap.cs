using UnityEngine;
using Gameplay.Simulation.Runtime;
using Gameplay.UI.Runtime;
using Metagame.UI.Runtime;
using UnityApplication = UnityEngine.Application;
using Company.Utilities.Runtime;
using Logger = Company.Utilities.Runtime.Logger;

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
            // Note: In general, I advocate for being quite strict about not using preprocessor directives in the codebase.
            // The only exception is in the initialization/bootstrap code. The alternative would be to create environment-dependent config files, which can be overkill at this point.
            // There's also some cases where we want to be strict during compilation, removing the possibility of configuration at runtime.
#if UNITY_EDITOR
            Logger.logger = new UnityLogger();
#endif
            
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