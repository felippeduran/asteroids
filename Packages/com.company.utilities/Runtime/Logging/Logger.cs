namespace Company.Utilities.Runtime
{
    public static class Logger
    {
        public static ILogger logger = new NullLogger();

        public static void Log(string message, params object[] args)
        {
            logger.Log(message, args);
        }
    }
}