using System.Linq;
using HtmlAgilityPack;

namespace BookCollector.Utilities
{
    public static class HtmlExtensions
    {
        public static HtmlNode SingleNodeWithAttributeNameAndValue(this HtmlNodeCollection source, string attribute_name, string attribute_value)
        {
            return source.SingleOrDefault(n => n.HasAttributes &&
                                          n.Attributes.Contains(attribute_name) &&
                                          n.Attributes[attribute_name].Value.ToLowerInvariant() == attribute_value);
        }

        public static string ChildLinkById(this HtmlDocument doc, string id)
        {
            var node = doc.GetElementbyId(id);
            var link_node = node.SelectSingleNode(".//a[@href]");
            return link_node.Attributes["href"].Value;
        }
    }
}
