using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityAddressables = UnityEngine.AddressableAssets.Addressables;

namespace Company.Utilities.Runtime
{
    public static class Addressables
    {
        public static async Task InitializeAsync()
        {
            await UnityAddressables.InitializeAsync().Task;
        }

        public static async Task<DisposableInstanceHandle<T>> InstantiateAsync<T>(this AssetReferenceGameObject assetReference, Transform parent = null, bool instantiateInWorldSpace = false) where T : MonoBehaviour
        {
            var obj = await assetReference.InstantiateAsync(parent, instantiateInWorldSpace).Task;
            var instance = obj.GetComponent<T>();
            return new DisposableInstanceHandle<T>(instance, assetReference);
        }

        public static async Task<T> GetPrefabAsync<T>(this AssetReferenceGameObject assetReference) where T : MonoBehaviour
        {
            GameObject asset;
            if (!assetReference.IsValid())
            {
                asset = await assetReference.LoadAssetAsync<GameObject>().Task;
            }
            else if (!assetReference.IsDone)
            {
                asset = await assetReference.OperationHandle.Task as GameObject;
            }
            else
            {
                asset = assetReference.Asset as GameObject;
            }
            var prefab = default(T);
            if (asset != null)
            {
                prefab = asset.GetComponent<T>();
            }
            return prefab;
        }

        public static async Task<T> LoadPrefabAsync<T>(this AssetReferenceGameObject assetReference) where T : MonoBehaviour
        {
            var prefab = default(T);
            GameObject asset = await assetReference.LoadAssetAsync<GameObject>().Task;
            if (asset != null)
            {
                prefab = asset.GetComponent<T>();
            }
            return prefab;
        }
    }
}