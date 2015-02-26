using System.ComponentModel.Composition;
using BookCollector.AmazonService;
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
            var client = new AWSECommerceServicePortTypeClient("AWSECommerceServicePort");
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(settings.AccessKeyId, settings.SecretKey));

            var item_search = new ItemSearch
            {
                Request = new[]
                {
                    new ItemSearchRequest
                    {
                        SearchIndex = "Books",
                        Title = "Orbus",
                        ResponseGroup = new[] { "ItemAttributes", "Images" }
                    }
                },
                AWSAccessKeyId = settings.AccessKeyId,
                AssociateTag = settings.AssociateTag
            };
            var response = client.ItemSearch(item_search);
        }
    }
}
