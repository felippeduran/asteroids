using System.Threading;
using System.Threading.Tasks;

namespace Metagame.UI.Runtime
{
    public interface IMainMenuView
    {
        Task<bool> WaitForCompletionAsync(CancellationToken ct);
    }

    public class MainMenuPresenter
    {
        readonly MetagameAssetLibrary metagameAssetLibrary;

        public MainMenuPresenter(MetagameAssetLibrary metagameAssetLibrary)
        {
            this.metagameAssetLibrary = metagameAssetLibrary;
        }

        public async Task<bool> PresentAsync(CancellationToken ct)
        {
            using var mainMenuView = await metagameAssetLibrary.CreateMainMenuViewAsync();

            return await mainMenuView.Obj.WaitForCompletionAsync(ct);
        }
    }
}