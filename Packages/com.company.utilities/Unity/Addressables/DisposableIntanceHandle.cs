using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Company.Utilities.Runtime;

namespace Company.Utilities.Unity
{
    public class DisposableInstanceHandle<TInstance> : IDisposableInstanceHandle<TInstance> where TInstance : MonoBehaviour
    {
        readonly TInstance obj;
        readonly AssetReferenceGameObject assetRef;

        public TInstance Obj => obj;
        public AssetReferenceGameObject AssetRef => assetRef;

        public DisposableInstanceHandle(TInstance obj, AssetReferenceGameObject assetRef)
        {
            this.obj = obj;
            this.assetRef = assetRef;
        }

        public void Dispose()
        {
            assetRef.ReleaseInstance(obj.gameObject);
        }
    }

    public class DisposableInstanceHandle<TInstance, TInterface> : IDisposableInstanceHandle<TInterface> where TInstance : MonoBehaviour, TInterface
    {
        readonly AssetReferenceGameObject assetRef;
        readonly TInstance obj;

        public TInterface Obj => obj;

        public static implicit operator DisposableInstanceHandle<TInstance, TInterface>(DisposableInstanceHandle<TInstance> handle)
        {
            return new DisposableInstanceHandle<TInstance, TInterface>(handle.Obj, handle.AssetRef);
        }

        public DisposableInstanceHandle(TInstance obj, AssetReferenceGameObject assetRef)
        {
            this.obj = obj;
            this.assetRef = assetRef;
        }

        public void Dispose()
        {
            assetRef.ReleaseInstance(obj.gameObject);
        }
    }
}