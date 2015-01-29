using System.IO;
using System.Linq;
using Caliburn.Micro;
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

        public static string ConvertHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        private static void ConvertTo(HtmlNode node, TextWriter out_text)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    node.ChildNodes.Apply(subnode => ConvertTo(subnode, out_text));
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    var parent_name = node.ParentNode.Name;
                    if ((parent_name == "script") || (parent_name == "style"))
                        break;

                    // get text
                    var html = ((HtmlTextNode) node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                        out_text.Write(HtmlEntity.DeEntitize(html));
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                            // treat paragraphs as crlf
                            out_text.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                        node.ChildNodes.Apply(subnode => ConvertTo(subnode, out_text));
                    break;
            }
        }
    }
}
