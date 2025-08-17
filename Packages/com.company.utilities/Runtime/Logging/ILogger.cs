namespace Company.Utilities.Runtime
{
    public interface ILogger
    {
        void Log(string message, params object[] args);
    }

    public class NullLogger : ILogger
    {
        public void Log(string message, params object[] args) { }
    }
}