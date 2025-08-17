using System;
using UnityEngine;

namespace Company.Utilities.Runtime
{
    public class BatchDisposer : IDisposable
    {
        readonly IDisposable[] objects;

        public BatchDisposer(params IDisposable[] objects)
        {
            this.objects = objects;
        }

        public void Dispose()
        {
            foreach (var obj in objects)
            {
                obj.Dispose();
            }
        }
    }

    public class DisposableGameObject : IDisposable
    {
        readonly GameObject gameObject;

        public DisposableGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public void Dispose()
        {
            GameObject.Destroy(gameObject);
        }
    }
}