using System;
using System.Diagnostics;

namespace BookCollector.Framework.Logging
{
    public class DebugLog : ILog
    {
        private readonly string type_name;

        public DebugLog(Type type)
        {
            type_name = type.FullName;
        }

        public void Info(string format, params object[] args)
        {
            Debug.WriteLine("[{1}] INFO: {0}", string.Format(format, args), type_name);
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine("[{1}] WARN: {0}", string.Format(format, args), type_name);
        }

        public void Error(Exception exception)
        {
            Debug.WriteLine("[{1}] ERROR: {0}", exception, type_name);
        }
    }
}
