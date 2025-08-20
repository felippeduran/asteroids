using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Company.Utilities.Runtime3
{

    public interface IViewController<TInstance>
    {
        Task PresentViewControllerAsync();
    }

    public class ViewController<TInstance>
    {
        readonly IAssetReference<TInstance> assetRef;

        TInstance view;
        IViewController<TInstance> presentedViewController;
        IViewController<TInstance> parentViewController;

        public TInstance View => view;

        public ViewHandle(IAssetReference<TInstance> assetRef)
        {
            this.assetRef = assetRef;
        }

        public async Task PresentViewControllerAsync(ViewController<TInstance> viewController)
        {
            viewController.LoadAsync();
            presentedViewController = viewController;
            // TODO: Present view controller on top of the current view controller
        }

        public async Task DismissViewControllerAsync(ViewController<TInstance> viewController)
        {
            if (viewController == presentedViewController)
            {
                await viewController.DismissImplementation(completion);
            }
        }

        public async Task DismissViewControllerAsync()
        {
            if (null != parentViewController)
            {
                await parentViewController.DismissViewController(this);
            }
        }

        public async Task LoadAsync()
        {
            if (view == null)
            {
                view = await assetRef.InstantiateAsync();
            }
        }

        private 

        public async Task<TExit> WaitForInputsAsync(TState state)
        {
            if (view == null)
            {
                view = await assetRef.InstantiateAsync();
            }
            return await view.PresentAsync(state);
        }

        public void Dispose()
        {
            assetRef.ReleaseInstance(view);
        }
    }

    public interface IAssetReference<TInstance>
    {
        Task<TInstance> InstantiateAsync();
        void ReleaseInstance(TInstance instance);
    }

    public class AssetReference<TInstance> : IAssetReference<TInstance> where TInstance : MonoBehaviour
    {
        readonly AssetReferenceGameObject assetRef;

        public async Task<TInstance> InstantiateAsync()
        {
            var instance = await assetRef.InstantiateAsync(parent: null).Task;
            var view = instance.GetComponent<TInstance>();
            view.gameObject.SetActive(true);
            return view;
        }

        public void ReleaseInstance(TInstance instance)
        {
            assetRef.ReleaseInstance(instance.gameObject);
        }
    }
}