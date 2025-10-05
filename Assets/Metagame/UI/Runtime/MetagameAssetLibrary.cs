using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Company.Utilities.Runtime;
using Company.Utilities.Unity;
using Metagame.Presentation.Runtime;

namespace Metagame.UI.Runtime
{
    [CreateAssetMenu(fileName = "MetagameAssetLibrary", menuName = "Metagame/MetagameAssetLibrary")]
    public class MetagameAssetLibrary : ScriptableObject, IMetagameUIAssetLibrary
    {
        [SerializeField] AssetReferenceGameObject mainMenuViewAsset;

        public async Task<IDisposableInstanceHandle<IMainMenuView>> CreateMainMenuViewAsync()
        {
            var mainMenuView = await mainMenuViewAsset.InstantiateAsync<MainMenuView>();
            mainMenuView.Obj.gameObject.SetActive(true);
            return (DisposableInstanceHandle<MainMenuView, IMainMenuView>)mainMenuView;
        }
    }
}