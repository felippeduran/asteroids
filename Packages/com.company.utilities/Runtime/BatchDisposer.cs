using System;

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
}