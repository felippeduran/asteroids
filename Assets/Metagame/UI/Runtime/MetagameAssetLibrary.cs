using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Company.Utilities.Runtime;
using Metagame.Presentation.Runtime;

namespace Metagame.UI.Runtime
{
    [CreateAssetMenu(fileName = "MetagameAssetLibrary", menuName = "Metagame/MetagameAssetLibrary")]
    public class MetagameAssetLibrary : ScriptableObject, IMetagameAssetLibrary
    {
        [SerializeField] AssetReferenceGameObject mainMenuViewAsset;

        public async Task<IDisposableInstanceHandle<IMainMenuView>> CreateMainMenuViewAsync()
        {
            var mainMenuView = await mainMenuViewAsset.InstantiateAsync<MainMenuView>();
            mainMenuView.Obj.gameObject.SetActive(true);
            return (IDisposableInstanceHandle<IMainMenuView>)mainMenuView;
        }
    }
}