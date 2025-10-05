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
}