using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Company.Utilities.Runtime;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime
{
    [CreateAssetMenu(fileName = "GameplayAssetLibrary", menuName = "Gameplay/GameplayAssetLibrary")]
    public class GameplayAssetLibrary : ScriptableObject
    {
        [SerializeField] AssetReferenceGameObject shipAsset;
        [SerializeField] AssetReferenceGameObject asteroidAsset;
        [SerializeField] AssetReferenceGameObject bulletAsset;
        [SerializeField] AssetReferenceGameObject saucerAsset;

        public async Task<GameplayAssets> LoadAllAsync()
        {
            var playerShipTask = shipAsset.LoadPrefabAsync<Ship>();
            var asteroidPrefabTask = asteroidAsset.LoadPrefabAsync<Asteroid>();
            var bulletPrefabTask = bulletAsset.LoadPrefabAsync<Bullet>();
            var saucerPrefabTask = saucerAsset.LoadPrefabAsync<Saucer>();

            await Task.WhenAll(playerShipTask, asteroidPrefabTask, bulletPrefabTask, saucerPrefabTask);

            var releaser = new AssetsReleaser(shipAsset, asteroidAsset, bulletAsset, saucerAsset);

            return new GameplayAssets(releaser)
            {
                Ship = playerShipTask.Result,
                Asteroid = asteroidPrefabTask.Result,
                Bullet = bulletPrefabTask.Result,
                Saucer = saucerPrefabTask.Result,
            };
        }

        public void ReleaseAll()
        {
            shipAsset.ReleaseAsset();
            asteroidAsset.ReleaseAsset();
            bulletAsset.ReleaseAsset();
            saucerAsset.ReleaseAsset();
        }
    }

    public class GameplayAssets : IDisposable
    {
        public Ship Ship { get; init; }
        public Asteroid Asteroid { get; init; }
        public Bullet Bullet { get; init; }
        public Saucer Saucer { get; init; }

        readonly IAssetsReleaser releaser;

        public GameplayAssets(IAssetsReleaser releaser)
        {
            this.releaser = releaser;
        }

        public void Dispose()
        {
            Logger.Log("Disposing gameplay asset references");
            releaser.ReleaseAll();
        }
    }
}