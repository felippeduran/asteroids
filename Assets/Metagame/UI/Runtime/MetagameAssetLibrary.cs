using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Company.Utilities.Runtime;

namespace Metagame.UI.Runtime
{
    [CreateAssetMenu(fileName = "MetagameAssetLibrary", menuName = "Metagame/MetagameAssetLibrary")]
    public class MetagameAssetLibrary : ScriptableObject
    {
        [SerializeField] AssetReferenceGameObject mainMenuViewAsset;

        public async Task<DisposableInstanceHandle<MainMenuView, IMainMenuView>> CreateMainMenuViewAsync()
        {
            var mainMenuView = await mainMenuViewAsset.InstantiateAsync<MainMenuView>();
            mainMenuView.Obj.gameObject.SetActive(true);
            return mainMenuView;
        }
    }
}