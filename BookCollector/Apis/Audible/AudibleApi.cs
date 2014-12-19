using System.ComponentModel.Composition;
using NLog;

namespace BookCollector.Apis.Audible
{
    [Export(typeof(AudibleApi))]
    public class AudibleApi : ApiBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override bool IsAuthenticated
        {
            get { return false; }
        }

        public AudibleApi() : base("Audible")
        {
        }
    }
}
