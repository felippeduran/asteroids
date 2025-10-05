using System;

namespace Company.Utilities.Runtime
{
    public interface IDisposableInstanceHandle<TInstance> : IDisposable
    {
        TInstance Obj { get; }
    }
}