using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void Disable();
    void Enable();
}

public class ObjectPool<T> : IDisposable where T : MonoBehaviour, IPoolable
{
    readonly T prefab;
    readonly List<T> available;
    readonly List<T> inUse;

    public ObjectPool(T prefab)
    {
        this.prefab = prefab;
        available = new List<T>();
        inUse = new List<T>();
    }

    public T Get()
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
            inUse.Add(obj);
        }

        obj.Enable();

        return obj;
    }

    public void Add(T obj)
    {
        obj.Disable();
        available.Add(obj);
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