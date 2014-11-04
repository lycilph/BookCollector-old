using System.Linq;
using System.Xml.Linq;
using RestSharp.Deserializers;
using RestSharp.Extensions;

namespace BookCollector.Utilities
{
    public class CustomDeserializer : XmlDeserializer
    {        
        protected override XAttribute GetAttributeByName(XElement root, XName name)
        {
            var lower_name = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
            var camel_name = name.LocalName.ToCamelCase(Culture).AsNamespaced(name.NamespaceName);

            if (root.Attribute(name) != null)
            {
                return root.Attribute(name);
            }

            if (root.Attribute(lower_name) != null)
            {
                return root.Attribute(lower_name);
            }

            if (root.Attribute(camel_name) != null)
            {
                return root.Attribute(camel_name);
            }

            // try looking for element that matches sanitized property name
            return root.Descendants()
                       .OrderBy(d => d.Ancestors().Count())
                       .Attributes()
                       .FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName) ??
                   root.Descendants()
                       .OrderBy(d => d.Ancestors().Count())
                       .Attributes()
                       .FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName.ToLower());
        }
    }
}
