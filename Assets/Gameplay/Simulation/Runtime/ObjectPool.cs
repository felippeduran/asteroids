using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void Disable();
    void Enable();
}

public class ObjectPool<T> where T : MonoBehaviour, IPoolable
{
    readonly T prefab;
    readonly List<T> pool;

    public ObjectPool(T prefab)
    {
        this.prefab = prefab;
        pool = new List<T>();
    }

    public T Get()
    {
        T obj;
        if (pool.Count > 0)
        {
            var lastIndex = pool.Count - 1;
            obj = pool[lastIndex];
            pool.RemoveAt(lastIndex);
        }
        else
        {
            obj = GameObject.Instantiate(prefab);
        }

        obj.Enable();

        return obj;
    }

    public void Add(T obj)
    {
        obj.Disable();
        pool.Add(obj);
    }
}