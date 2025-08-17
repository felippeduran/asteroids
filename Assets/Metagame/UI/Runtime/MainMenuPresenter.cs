using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Metagame.UI.Runtime
{
    public class MainMenuPresenter
    {
        readonly MetagameViewLibrary metagameViewLibrary;

        public MainMenuPresenter(MetagameViewLibrary metagameViewLibrary)
        {
            this.metagameViewLibrary = metagameViewLibrary;
        }

        public async Task<bool> PresentAsync(CancellationToken ct)
        {
            using var mainMenuView = metagameViewLibrary.CreateMainMenuView();

            return await mainMenuView.WaitForCompletionAsync(ct);
        }
    }
}