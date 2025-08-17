using UnityEngine;

namespace Company.Utilities.Runtime
{
    // Note: Ideally, this UnityLogger should be in a separate assembly, so that there's no transitive dependency on UnityEngine.
    public class UnityLogger : ILogger
    {
        public void Log(string message, params object[] args)
        {
            // Note: Running the format inside the log function can save unecessary string allocations when log is disabled.
            // We can also use ConditionalAttribute to strip the log call during compilation when logging is disabled.
            Debug.Log(string.Format(message, args));
        }
    }
}