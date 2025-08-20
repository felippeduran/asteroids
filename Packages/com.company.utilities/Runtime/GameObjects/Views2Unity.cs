using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Company.Utilities.Runtime2.UnityViews
{
    public class UnityAssetReference<TInstance> : IAssetReference<TInstance> where TInstance : MonoBehaviour
    {
        readonly AssetReferenceGameObject assetRef;

        public UnityAssetReference(AssetReferenceGameObject assetRef)
        {
            this.assetRef = assetRef;
        }

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

    public class CanvasView : View
    {
        [SerializeField] Canvas canvas;
        [SerializeField] new Camera camera;

        public override void AddAsSubviewOf(IView parentView)
        {
            // TODO: Add as subview of the view handle
            var view = parentView as View;
            if (view != null)
            {
                camera.depth = view.GetDepth() + 1f;
            }
        }

        public override float GetDepth()
        {
            return camera.depth;
        }
    }

    public class RegularView : View
    {
        float depth;

        public override void AddAsSubviewOf(IView parentView)
        {
            var view = parentView as View;
            if (view != null)
            {
                transform.SetParent(view.transform);
            }
        }

        public override float GetDepth()
        {
            return depth;
        }
    }

    public abstract class View : MonoBehaviour, IView
    {
        public abstract void AddAsSubviewOf(IView view);
        public abstract float GetDepth();
    }

    // public class UIOrchestrator
    // {
    //     readonly List<ViewController<MonoBehaviour>> views = new List<ViewController<MonoBehaviour>>();

    //     public void AddView(ViewController<MonoBehaviour> view)
    //     {
    //         views.Add(view);
    //     }
    // }

    // public class UnityViewHandle<TInstance> : IViewHandle<TInstance> where TInstance : MonoBehaviour
    // {
    //     readonly UnityAssetReference<TInstance> assetRef;

    //     object IViewHandle.View => View;
    //     public TInstance View { get; private set; }

    //     public UnityViewHandle(UnityAssetReference<TInstance> assetRef)
    //     {
    //         this.assetRef = assetRef;
    //     }

    //     public async Task LoadAsync()
    //     {
    //         if (View == null)
    //         {
    //             View = await assetRef.InstantiateAsync();
    //         }
    //     }

    //     public void AddAsSubviewOf(IViewHandle childViewHandle)
    //     {
    //         // TODO: Add as subview of the view handle
    //     }

    //     public void Dispose()
    //     {
    //         assetRef.ReleaseInstance(View);
    //     }
    // }

    // public class UnityViewHandle<TInstance> : IViewHandle<TInstance> where TInstance : MonoBehaviour
    // {
    //     readonly UnityAssetReference<TInstance> assetRef;

    //     object IViewHandle.View => View;
    //     public TInstance View { get; private set; }

    //     public UnityViewHandle(UnityAssetReference<TInstance> assetRef)
    //     {
    //         this.assetRef = assetRef;
    //     }

    //     public async Task LoadAsync()
    //     {
    //         if (View == null)
    //         {
    //             View = await assetRef.InstantiateAsync();
    //         }
    //     }

    //     public void AddAsSubviewOf(IViewHandle childViewHandle)
    //     {
    //         // TODO: Add as subview of the view handle
    //     }

    //     public void Dispose()
    //     {
    //         assetRef.ReleaseInstance(View);
    //     }
    // }

    public class UnityPersistentViewHandle<TInstance> : IViewHandle<TInstance> where TInstance : MonoBehaviour, IView
    {
        IView IViewHandle.View => View;
        public TInstance View { get; private set; }

        public UnityPersistentViewHandle(TInstance view)
        {
            View = view;
        }

        public Task LoadAsync()
        {
            View.gameObject.SetActive(true);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            View.gameObject.SetActive(false);
        }
    }
}