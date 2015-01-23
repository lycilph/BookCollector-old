using System;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Utilities;
using HtmlAgilityPack;

namespace BookCollector.Apis.Audible
{
    [Export(typeof(IApi))]
    [Export(typeof(AudibleApi))]
    public class AudibleApi : IApi
    {
        public string Name { get { return "Audible"; } }

        public AudibleBook Parse(HtmlNode node)
        {
            var inputs = node.SelectNodes(".//input");
            if (inputs == null) return null;

            var asin_node = inputs.SingleNodeWithAttributeNameAndValue("name", "asin");
            if (asin_node == null) return null;

            var parent_asin_node = inputs.SingleNodeWithAttributeNameAndValue("name", "parentasin");
            if (parent_asin_node == null) return null;

            var title_node = node.SelectSingleNode(".//a[@name='tdTitle']");
            if (title_node == null) return null;

            var description_node = node.SelectSingleNode(".//p");
            if (description_node == null) return null;

            var list = node.SelectNodes(".//strong");
            if (list == null) return null;

            var product_cover_node = node.SelectSingleNode(".//td[@name='productCover']");
            var image_node = (product_cover_node != null ? product_cover_node.SelectSingleNode(".//img") : null);

            var parent_asin = parent_asin_node.Attributes["value"].Value;
            var asin = asin_node.Attributes["value"].Value;

            var authors = list[0].InnerText;
            var authors_list = authors.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToList();

            var narrators = list[1].InnerText;
            var narrators_list = narrators.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.Trim())
                .ToList();

            return new AudibleBook
            {
                Title = title_node.InnerText,
                Asin = asin,
                ParentAsin = parent_asin,
                Authors = authors_list,
                Narrators = narrators_list,
                Description = description_node.InnerText.Trim(),
                ImageUrl = (image_node == null ? "" : image_node.Attributes["src"].Value)
            };
        }
    }
}
