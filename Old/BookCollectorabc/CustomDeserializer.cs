using System.Linq;
using System.Xml.Linq;
using RestSharp.Deserializers;
using RestSharp.Extensions;

namespace BookCollector
{
    public class CustomDeserializer : XmlDeserializer
    {        
        protected override XAttribute GetAttributeByName(XElement root, XName name)
        {
            var lowerName = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
            var camelName = name.LocalName.ToCamelCase(Culture).AsNamespaced(name.NamespaceName);

            if (root.Attribute(name) != null)
            {
                return root.Attribute(name);
            }

            if (root.Attribute(lowerName) != null)
            {
                return root.Attribute(lowerName);
            }

            if (root.Attribute(camelName) != null)
            {
                return root.Attribute(camelName);
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
