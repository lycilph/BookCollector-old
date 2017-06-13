using BookCollector.Framework.Logging;

namespace BookCollector.Domain
{
    public class ApplicationObjectMapping
    {
        public static void Setup()
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Info("Configuring Mapper");
        }
    }
}
