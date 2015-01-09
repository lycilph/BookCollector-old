using System.ComponentModel.Composition;
using NLog;

namespace BookCollector.Apis.Audible
{
    [Export(typeof(IApi))]
    [Export(typeof(AudibleApi))]
    public class AudibleApi : IApi
    {
        public string Name { get { return "Audible"; } }
    }
}
