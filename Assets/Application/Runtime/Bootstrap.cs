using UnityEngine;
using Gameplay.Simulation.Runtime;
using Gameplay.Simulation.Unity;
using Gameplay.UI.Runtime;
using Metagame.UI.Runtime;
using Metagame.Presentation.Runtime;
using Gameplay.Presentation.Runtime;
using Company.Utilities.Unity;
using UnityApplication = UnityEngine.Application;
using Logger = Company.Utilities.Runtime.Logger;

namespace Application.Runtime
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] GameConfigAsset gameConfigAsset;
        [SerializeField] GameplayAssetLibrary gameplayAssetLibrary;
        [SerializeField] GameplayUIAssetLibrary gameplayUIAssetLibrary;
        [SerializeField] MetagameAssetLibrary metagameAssetLibrary;

        async void Start()
        {
            // Note: In general, I advocate for being quite strict about not using preprocessor directives in the codebase.
            // The only exception is in the initialization/bootstrap code. The alternative would be to create environment-dependent config files, which can be overkill at this point.
            // There's also some cases where we want to be strict during compilation, removing the possibility of configuration at runtime.
#if UNITY_EDITOR
            Logger.logger = new UnityLogger();
#endif

            Screen.SetResolution(Screen.height, Screen.height, false);

            await Addressables.InitializeAsync();

            var exitToken = UnityApplication.exitCancellationToken;
            while (!exitToken.IsCancellationRequested)
            {
                var mainMenuPresenter = new MainMenuPresenter(metagameAssetLibrary);
                var play = await mainMenuPresenter.PresentAsync(exitToken);
                if (play)
                {
                    var physicsFactory = new PhysicsFactory();
                    var gameplayFactory = new GameplayFactory(gameplayAssetLibrary, physicsFactory, gameConfigAsset.GameConfig);

                    await new GameplayPresenter(gameplayUIAssetLibrary, gameplayFactory).PresentAsync(exitToken);
                }
            }
        }
    }
}