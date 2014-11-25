using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using HtmlAgilityPack;

namespace BookCollector.Utilities
{
    public static class HtmlExtensions
    {
        public static IEnumerable<HtmlNode> WithAttributes(this IEnumerable<HtmlNode> nodes, params string[] names)
        {
            return nodes.Where(n => n.HasAttributes && names.All(n.Attributes.Contains));
        }
    }
}
