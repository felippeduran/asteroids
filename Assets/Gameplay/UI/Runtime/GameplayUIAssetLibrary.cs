using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Company.Utilities.Runtime;

namespace Gameplay.UI.Runtime
{
    [CreateAssetMenu(fileName = "GameplayUIAssetLibrary", menuName = "Gameplay/GameplayUIAssetLibrary")]
    public class GameplayUIAssetLibrary : ScriptableObject
    {
        [SerializeField] AssetReferenceGameObject camerasAsset;
        [SerializeField] AssetReferenceGameObject gameplayViewAsset;
        [SerializeField] AssetReferenceGameObject gameOverViewAsset;

        public async Task<DisposableInstanceHandle<CameraGroup, ICameraGroup>> CreateCameraGroupAsync()
        {
            var cameras = await camerasAsset.InstantiateAsync<CameraGroup>();
            cameras.Obj.gameObject.SetActive(true);
            return cameras;
        }

        public async Task<DisposableInstanceHandle<GameOverView, IGameOverView>> CreateGameOverViewAsync()
        {
            var gameOverView = await gameOverViewAsset.InstantiateAsync<GameOverView>();
            gameOverView.Obj.gameObject.SetActive(true);
            return gameOverView;
        }

        public async Task<DisposableInstanceHandle<GameplayView, IGameplayView>> CreateGameplayViewAsync()
        {
            var gameplayView = await gameplayViewAsset.InstantiateAsync<GameplayView>();
            gameplayView.Obj.gameObject.SetActive(true);
            return gameplayView;
        }
    }
}