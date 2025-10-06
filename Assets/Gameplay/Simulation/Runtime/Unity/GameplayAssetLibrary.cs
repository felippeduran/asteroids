using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Gameplay.Simulation.Runtime;
using Company.Utilities.Runtime;
using Company.Utilities.Runtime.Unity;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime.Unity
{
    [CreateAssetMenu(fileName = "GameplayAssetLibrary", menuName = "Gameplay/GameplayAssetLibrary")]
    public class GameplayAssetLibrary : ScriptableObject, IGameplayAssetLibrary
    {
        [SerializeField] AssetReferenceGameObject shipAsset;
        [SerializeField] AssetReferenceGameObject asteroidAsset;
        [SerializeField] AssetReferenceGameObject bulletAsset;
        [SerializeField] AssetReferenceGameObject saucerAsset;

        public async Task<IGameplayAssets> LoadAllAsync()
        {
            var playerShipTask = shipAsset.LoadPrefabAsync<Ship>();
            var asteroidPrefabTask = asteroidAsset.LoadPrefabAsync<Asteroid>();
            var bulletPrefabTask = bulletAsset.LoadPrefabAsync<Bullet>();
            var saucerPrefabTask = saucerAsset.LoadPrefabAsync<Saucer>();

            await Task.WhenAll(playerShipTask, asteroidPrefabTask, bulletPrefabTask, saucerPrefabTask);

            var releaser = new AssetsReleaser(shipAsset, asteroidAsset, bulletAsset, saucerAsset);

            var shipsPool = new ObjectPool<IShip, Ship>(playerShipTask.Result);
            var asteroidsPool = new ObjectPool<IAsteroid, Asteroid>(asteroidPrefabTask.Result);
            var bulletsPool = new ObjectPool<IBullet, Bullet>(bulletPrefabTask.Result);
            var saucersPool = new ObjectPool<ISaucer, Saucer>(saucerPrefabTask.Result);

            var disposer = new BatchDisposer(shipsPool, asteroidsPool, bulletsPool, saucersPool);

            return new GameplayAssets(releaser, disposer)
            {
                Ships = shipsPool,
                Asteroids = asteroidsPool,
                Bullets = bulletsPool,
                Saucers = saucersPool,
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

    public class GameplayAssets : IGameplayAssets,IDisposable
    {
        public IObjectPool<IShip> Ships { get; init; }
        public IObjectPool<IAsteroid> Asteroids { get; init; }
        public IObjectPool<IBullet> Bullets { get; init; }
        public IObjectPool<ISaucer> Saucers { get; init; }

        readonly IAssetsReleaser releaser;
        readonly IDisposable disposer;

        public GameplayAssets(IAssetsReleaser releaser, IDisposable disposer)
        {
            this.releaser = releaser;
            this.disposer = disposer;
        }

        public IGameplay CreateGameplay(GameState gameState, GameConfig gameConfig, GameSystems gameSystems)
        {
            var gameLoopObject = new GameObject("GameLoop");
            var bootstrap = gameLoopObject.AddComponent<GameplayLoop>();

            bootstrap.Setup(gameState, gameConfig, gameSystems, this);

            return bootstrap;
        }

        public void Dispose()
        {
            Logger.Log("Disposing gameplay asset references");
            disposer.Dispose();
            releaser.ReleaseAll();
        }
    }
}