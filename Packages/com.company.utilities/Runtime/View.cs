using System;
using UnityEngine;

namespace Company.Utilities.Runtime
{
    public abstract class View : MonoBehaviour, IDisposable
    {
        public virtual void Dispose()
        {
            Destroy(gameObject);
        }
    }
}