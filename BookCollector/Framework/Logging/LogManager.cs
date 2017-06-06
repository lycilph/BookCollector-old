using System;
using System.Diagnostics;

namespace BookCollector.Framework.Logging
{
    public class LogManager
    {
        static readonly ILog NullLogInstance = new NullLog();

        public static Func<Type, ILog> GetLog = type => NullLogInstance;

        public static ILog GetCurrentClassLogger()
        {
            Type declaring_type;
            int frames_to_skip = 1;

            do
            {
                var frame = new StackFrame(frames_to_skip, false);
                var method = frame.GetMethod();
                declaring_type = method.DeclaringType;
                if (declaring_type == null)
                {
                    throw new InvalidOperationException("Cannot find current class logger");
                }

                frames_to_skip++;
            } while (declaring_type.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return GetLog(declaring_type);
        }
    }
}
