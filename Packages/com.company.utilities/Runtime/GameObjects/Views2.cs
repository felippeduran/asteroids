using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Company.Utilities.Runtime2
{
    public interface IAssetReference<TInstance>
    {
        Task<TInstance> InstantiateAsync();
        void ReleaseInstance(TInstance instance);
    }

    public interface IView
    {
        void AddAsSubviewOf(IView view);
    }

    public interface IView<TState, TEvents> : IView
    {
        Task<TEvents> WaitForEventsAsync(TState state, CancellationToken ct);
    }

    public interface IView<TState> : IView
    {
        Task UpdateAsync(TState state, CancellationToken ct);
    }

    public interface ISimpleView : IView
    {
        Task WaitForCompletionAsync(CancellationToken ct);
    }

    public interface IViewHandle : IDisposable
    {
        IView View { get; }
        Task LoadAsync();
    }

    public interface IViewHandle<TInstance> : IViewHandle where TInstance : IView
    {
        new TInstance View { get; }
    }

    public class ViewHandle<TInstance> : IViewHandle<TInstance> where TInstance : IView
    {
        readonly IAssetReference<TInstance> assetRef;

        IView IViewHandle.View => View;
        public TInstance View { get; private set; }

        public ViewHandle(IAssetReference<TInstance> assetRef)
        {
            this.assetRef = assetRef;
        }

        public async Task LoadAsync()
        {
            if (View == null)
            {
                View = await assetRef.InstantiateAsync();
            }
        }

        public void Dispose()
        {
            assetRef.ReleaseInstance(View);
        }
    }

    // public interface IViewHandle<TInstance, TState, TEvents> : IViewHandle<TInstance> where TInstance : IView<TState, TEvents>
    // {
    //     // Task<TEvents> WaitForEventsAsync(TState state);
    // }

    // public abstract class ViewHandle<TInstance, TEvents, TState> : ViewHandle<TInstance> where TInstance : IView<TEvents, TState>
    // {
    //     public ViewHandle(IAssetReference<TInstance> assetRef) : base(assetRef) { }

    //     public async Task<TEvents> WaitForEventsAsync(TState state)
    //     {
    //         if (view == null)
    //         {
    //             view = await assetRef.InstantiateAsync();
    //         }
    //         return await view.PresentAsync(state);
    //     }
    // }

    // public abstract class ViewHandle<TInstance, TEvents, TState> : ViewHandle<TInstance> where TInstance : IView<TEvents, TState>
    // {
    //     public ViewHandle(IAssetReference<TInstance> assetRef) : base(assetRef) { }

    //     public async Task<TEvents> WaitForEventsAsync(TState state)
    //     {
    //         if (view == null)
    //         {
    //             view = await assetRef.InstantiateAsync();
    //         }
    //         return await view.PresentAsync(state);
    //     }
    // }

    public interface IViewController : IDisposable
    {
        IViewHandle ViewHandle { get; }
        Task LoadAsync();
        Task PresentViewAsync(IViewController viewController);
    }

    public interface IViewController<TState, TEvents> : IViewController
    {
        Task<TEvents> WaitForEventsAsync(TState state, CancellationToken ct);
    }

    public interface IViewController<TState> : IViewController
    {
        Task UpdateAsync(TState state, CancellationToken ct);
    }

    public interface ISimpleViewController : IViewController
    {
        Task WaitForCompletionAsync(CancellationToken ct);
    }

    public abstract class ViewController<TInstance> : IViewController where TInstance : IView
    {
        protected readonly IViewHandle<TInstance> viewHandle;

        public IViewHandle ViewHandle => viewHandle;

        public ViewController(IViewHandle<TInstance> viewHandle)
        {
            this.viewHandle = viewHandle;
        }

        public async Task PresentViewAsync(IViewController viewController)
        {
            await viewController.ViewHandle.LoadAsync();
            viewHandle.View.AddAsSubviewOf(viewController.ViewHandle.View);
        }

        public async Task LoadAsync()
        {
            await viewHandle.LoadAsync();
        }

        public void Dispose()
        {
            viewHandle.Dispose();
        }
    }

    public class ViewController<TInstance, TState, TEvents> : ViewController<TInstance>, IViewController<TState, TEvents> where TInstance : IView<TState, TEvents>
    {
        public ViewController(IViewHandle<TInstance> viewHandle) : base(viewHandle) { }

        public async Task<TEvents> WaitForEventsAsync(TState state, CancellationToken ct)
        {
            if (viewHandle.View == null)
            {
                await LoadAsync();
            }
            return await viewHandle.View.WaitForEventsAsync(state, ct);
        }
    }

    public class ViewController<TInstance, TState> : ViewController<TInstance>, IViewController<TState> where TInstance : IView<TState>
    {
        public ViewController(IViewHandle<TInstance> viewHandle) : base(viewHandle) { }

        public async Task UpdateAsync(TState state, CancellationToken ct)
        {
            if (viewHandle.View == null)
            {
                await LoadAsync();
            }
            await viewHandle.View.UpdateAsync(state, ct);
        }
    }

    public class SimpleViewController<TInstance> : ViewController<TInstance>, ISimpleViewController where TInstance : ISimpleView
    {
        public SimpleViewController(IViewHandle<TInstance> viewHandle) : base(viewHandle) { }

        public async Task WaitForCompletionAsync(CancellationToken ct)
        {
            if (viewHandle.View == null)
            {
                await LoadAsync();
            }
            await viewHandle.View.WaitForCompletionAsync(ct);
        }
    }

    // public abstract class ViewHandle<TInstance> : IViewHandle<TInstance>
    // {
    //     protected readonly IAssetReference<TInstance> assetRef;
    //     protected TInstance view;

    //     object IViewHandle.View => view;
    //     public TInstance View => view;

    //     public ViewHandle(IAssetReference<TInstance> assetRef)
    //     {
    //         this.assetRef = assetRef;
    //     }

    //     public async Task PresentViewAsync(IViewHandle<TInstance> viewHandle)
    //     {
    //         await viewHandle.LoadAsync();
    //         viewHandle.AddAsSubviewOf(this);
    //     }

    //     public abstract void AddAsSubviewOf(IViewHandle childViewHandle);

    //     public async Task LoadAsync()
    //     {
    //         if (view == null)
    //         {
    //             view = await assetRef.InstantiateAsync();
    //         }
    //     }

    //     public void Dispose()
    //     {
    //         assetRef.ReleaseInstance(view);
    //     }
    // }

    public interface ILoadingView : IView<float>
    {
        
    }

    public interface IWireframe : IDisposable
    {
        IViewController LoadingViewController { get; }
        IViewController RootViewController { get; }

        Task PresentViewAsync(IViewController viewController);
        Task<bool> PresentLoadingViewAsync(Task[] loadTasks, CancellationToken ct);
    }

    public class Wireframe : IWireframe
    {
        readonly IViewController<float> loadingVc;
        readonly ISimpleViewController rootView;

        public Wireframe(ViewHandle<ISimpleView> rootView, ViewHandle<ILoadingView> loadingVc)
        {
            this.loadingVc = new ViewController<ILoadingView, float>(loadingVc);
            this.rootView = new SimpleViewController<ISimpleView>(rootView);
        }

        public async Task PresentViewAsync(IViewController viewController)
        {
            await rootView.PresentViewAsync(viewController);
        }

        public async Task<bool> PresentLoadingViewAsync(Task[] loadTasks, CancellationToken ct)
        {
            using (loadingVc)
            {
                await rootView.PresentViewAsync(loadingVc);
                while (!ct.IsCancellationRequested)
                {
                    await Task.WhenAny(loadTasks);
                    var progress = (float)loadTasks.Sum(t => t.IsCompleted ? 1 : 0) / loadTasks.Length;
                    await loadingVc.UpdateAsync(progress, ct);
                }
            }

            return ct.IsCancellationRequested;
        }

        public void Dispose()
        {
            rootView.Dispose();
            loadingVc.Dispose();
        }
    }
}