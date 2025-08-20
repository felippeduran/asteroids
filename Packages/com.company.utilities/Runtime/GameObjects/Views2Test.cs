using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Company.Utilities.Runtime2.UnityViews;
using Company.Utilities.Runtime2;
using System.Threading;
using UnityEngine.UI;

namespace Company.Utilities.Runtime2.Test
{
    public struct XEventData {
        public bool OpenY;
    }

    public struct XStateData {
        public float Value;
    }

    public class XView : CanvasView, IView<XStateData, XEventData>
    {
        public async Task<XEventData> WaitForEventsAsync(XStateData state, CancellationToken ct)
        {
            return await Task.FromResult(new XEventData());
        }
    }

    public struct YEventData {
        public bool Exit;
    }

    public struct YStateData {
        public float Value;
    }

    public class YView : CanvasView, IView<YStateData, YEventData>
    {
        public async Task<YEventData> WaitForEventsAsync(YStateData state, CancellationToken ct)
        {
            return await Task.FromResult(new YEventData());
        }
    }

    public class Test : MonoBehaviour
    {
        [SerializeField] AssetReferenceGameObject addressableRefX;
        [SerializeField] AssetReferenceGameObject addressableRefY;

        public async void Start()
        {
            var exitToken = Application.exitCancellationToken;

            var assetRefX = new UnityAssetReference<XView>(addressableRefX);
            var assetRefY = new UnityAssetReference<YView>(addressableRefY);
            var viewControllerFactory = new UnityViewControllerFactory(assetRefX, assetRefY);

            Wireframe wireframe = null;

            using var x = viewControllerFactory.CreateXVc();
            using var y = viewControllerFactory.CreateYVc();

            await wireframe.PresentLoadingViewAsync(new Task[] { x.LoadAsync(), y.LoadAsync() }, exitToken);

            while (!exitToken.IsCancellationRequested)
            {
                var xEventData = await x.WaitForEventsAsync(new XStateData { Value = 1 }, exitToken);

                if (xEventData.OpenY)
                {
                    using var viewController = viewControllerFactory.CreateYVc();

                    using ViewController<YView, YStateData, YEventData> vc = viewControllerFactory.CreateYVc();

                    await wireframe.PresentViewAsync(viewController);

                    while (!exitToken.IsCancellationRequested)
                    {
                        var cts = CancellationTokenSource.CreateLinkedTokenSource(exitToken).Token;
                        YEventData yEventData = await vc.WaitForEventsAsync(new YStateData(), cts);

                        if (yEventData.Exit)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }


    public class UnityViewControllerFactory
    {
        readonly UnityAssetReference<XView> assetRefX;
        readonly UnityAssetReference<YView> assetRefY;

        public UnityViewControllerFactory(UnityAssetReference<XView> assetRefX, UnityAssetReference<YView> assetRefY)
        {
            this.assetRefX = assetRefX;
            this.assetRefY = assetRefY;
        }

        public ViewController<XView, XStateData, XEventData> CreateXVc()
        {
            var viewHandle = new ViewHandle<XView>(assetRefX);
            return new ViewController<XView, XStateData, XEventData>(viewHandle);
        }

        public ViewController<YView, YStateData, YEventData> CreateYVc()
        {
            var viewHandle = new ViewHandle<YView>(assetRefY);
            return new ViewController<YView, YStateData, YEventData>(viewHandle);
        }
    }

    public class LoadingView : CanvasView, IView<float>
    {
        [SerializeField] Slider progressBar;

        public Task UpdateAsync(float progress, CancellationToken ct)
        {
            progressBar.value = progress;
            return Task.CompletedTask;
        }
    }
}