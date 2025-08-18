using System;
using System.Collections.Generic;
using UnityEngine;

namespace Company.Utilities.Runtime
{
    public interface IPoolable
    {
        void Disable();
        void Enable();
    }

    public interface IObjectPool<T> where T : IPoolable
    {
        T Get();
        void Add(T obj);
    }

    public class ObjectPool<TInterface, T> : IObjectPool<TInterface>, IDisposable
    where T : MonoBehaviour, TInterface
    where TInterface : IPoolable
    {
        readonly T prefab;
        readonly List<T> available;
        readonly HashSet<T> inUse;

        public ObjectPool(T prefab)
        {
            this.prefab = prefab;
            available = new List<T>();
            inUse = new HashSet<T>();
        }

        public TInterface Get()
        {
            T obj;
            if (available.Count > 0)
            {
                var lastIndex = available.Count - 1;
                obj = available[lastIndex];
                available.RemoveAt(lastIndex);
            }
            else
            {
                obj = GameObject.Instantiate(prefab);
            }

            inUse.Add(obj);
            obj.Enable();

            return obj;
        }

        public void Add(TInterface obj)
        {
            obj.Disable();
            available.Add(obj as T);
            inUse.Remove(obj as T);

        }

        public void Dispose()
        {
            foreach (var obj in available)
            {
                GameObject.Destroy(obj.gameObject);
            }

            foreach (var obj in inUse)
            {
                GameObject.Destroy(obj.gameObject);
            }
        }
    }
}