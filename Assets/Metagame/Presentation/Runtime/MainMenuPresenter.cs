using System.Threading;
using System.Threading.Tasks;
using Company.Utilities.Runtime;

namespace Metagame.Presentation.Runtime
{
    public interface IMetagameAssetLibrary
    {
        Task<IDisposableInstanceHandle<IMainMenuView>> CreateMainMenuViewAsync();
    }

    public interface IMainMenuView
    {
        Task<bool> WaitForCompletionAsync(CancellationToken ct);
    }

    public class MainMenuPresenter
    {
        readonly IMetagameAssetLibrary metagameAssetLibrary;

        public MainMenuPresenter(IMetagameAssetLibrary metagameAssetLibrary)
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