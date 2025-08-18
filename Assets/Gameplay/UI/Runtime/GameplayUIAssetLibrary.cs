using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Gameplay.Simulation.Runtime;
using Gameplay.Presentation.Runtime;
using Company.Utilities.Runtime;

namespace Gameplay.UI.Runtime
{
    [CreateAssetMenu(fileName = "GameplayUIAssetLibrary", menuName = "Gameplay/GameplayUIAssetLibrary")]
    public class GameplayUIAssetLibrary : ScriptableObject, IGameplayAssetLibrary
    {
        [SerializeField] AssetReferenceGameObject camerasAsset;
        [SerializeField] AssetReferenceGameObject gameplayViewAsset;
        [SerializeField] AssetReferenceGameObject gameOverViewAsset;

        public async Task<IDisposableInstanceHandle<ICameraGroup>> CreateCameraGroupAsync()
        {
            var cameras = await camerasAsset.InstantiateAsync<CameraGroup>();
            cameras.Obj.gameObject.SetActive(true);
            return (DisposableInstanceHandle<CameraGroup, ICameraGroup>)cameras;
        }

        public async Task<IDisposableInstanceHandle<IGameOverView>> CreateGameOverViewAsync()
        {
            var gameOverView = await gameOverViewAsset.InstantiateAsync<GameOverView>();
            gameOverView.Obj.gameObject.SetActive(true);
            return (DisposableInstanceHandle<GameOverView, IGameOverView>)gameOverView;
        }

        public async Task<IDisposableInstanceHandle<IGameplayView>> CreateGameplayViewAsync()
        {
            var gameplayView = await gameplayViewAsset.InstantiateAsync<GameplayView>();
            gameplayView.Obj.gameObject.SetActive(true);
            return (DisposableInstanceHandle<GameplayView, IGameplayView>)gameplayView;
        }
    }
}