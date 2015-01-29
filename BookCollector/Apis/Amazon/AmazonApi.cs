using System.ComponentModel.Composition;
using BookCollector.Services;

namespace BookCollector.Apis.Amazon
{
    [Export(typeof(IApi))]
    [Export(typeof(AmazonApi))]
    public class AmazonApi : IApi
    {
        private readonly AmazonSettings settings;

        public string Name { get { return "Amazon"; } }

        [ImportingConstructor]
        public AmazonApi(ApplicationSettings application_settings)
        {
            settings = application_settings.GetSettings<AmazonSettings>(Name);
        }

        public void Search(string isbn)
        {
            
        }
    }
}
