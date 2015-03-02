using System.ServiceModel.Channels;
using System.Xml;

namespace Test
{
    public class AmazonHeader : MessageHeader
    {
        private readonly string name;
        private readonly string value;

        public override string Name { get { return name; } }
        public override string Namespace { get { return "http://security.amazonaws.com/doc/2007-01-01/"; } }

        public AmazonHeader(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion version)
        {
            writer.WriteString(value);
        }
    }
}
