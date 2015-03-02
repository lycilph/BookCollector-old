using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Test
{
    public class AmazonSigningEndpointBehavior : IEndpointBehavior
    {
        private readonly string access_key_id = "";
        private readonly string secret_key = "";

        public AmazonSigningEndpointBehavior(string access_key_id, string secret_key)
        {
            this.access_key_id = access_key_id;
            this.secret_key = secret_key;
        }

        public void ApplyClientBehavior(ServiceEndpoint service_endpoint, ClientRuntime client_runtime)
        {
            client_runtime.ClientMessageInspectors.Add(new AmazonSigningMessageInspector(access_key_id, secret_key));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint service_endpoint, EndpointDispatcher endpoint_dispatched) { }

        public void Validate(ServiceEndpoint service_endpoint) { }

        public void AddBindingParameters(ServiceEndpoint service_endpoint, BindingParameterCollection binding_paremeters) { }
    }
}
