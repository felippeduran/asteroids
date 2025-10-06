using System;
using UnityEngine;

namespace Company.Utilities.Runtime.Unity
{
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