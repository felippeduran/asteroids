using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Metagame.UI.Runtime
{
    public class MainMenuPresenter
    {
        readonly MetagameViewLibrary mainMenuViewLibrary;

        public MainMenuPresenter(MetagameViewLibrary mainMenuViewLibrary)
        {
            this.mainMenuViewLibrary = mainMenuViewLibrary;
        }

        public async Task PresentAsync(CancellationToken ct)
        {
            var mainMenuView = GameObject.Instantiate(mainMenuViewLibrary.MainMenuViewPrefab);

            await mainMenuView.WaitForCompletionAsync(ct);

            GameObject.Destroy(mainMenuView.gameObject);
        }
    }
}