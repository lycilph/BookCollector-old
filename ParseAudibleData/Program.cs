using System;
using System.Linq;
using HtmlAgilityPack;

namespace ParseAudibleData
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            doc.Load(@"c:\users\kbn_mml\downloads\final.html");

            var content = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'adbl-lib-content')]");
            foreach (var node in content.SelectNodes(".//tr"))
            {
                if (!node.HasChildNodes) continue;

                var inputs = node.SelectNodes(".//input");
                if (inputs == null) continue;

                var asin_node = inputs.SingleOrDefault(i => i.HasAttributes && i.Attributes.Contains("name") && i.Attributes["name"].Value.ToLowerInvariant() == "asin");
                if (asin_node == null) continue;

                var parent_asin_node = inputs.SingleOrDefault(i => i.HasAttributes && i.Attributes.Contains("name") && i.Attributes["name"].Value.ToLowerInvariant() == "parentasin");
                if (parent_asin_node == null) continue;

                var title_node = node.SelectNodes(".//a[@name]").SingleOrDefault(n => n.Attributes["name"].Value.ToLowerInvariant() == "tdtitle");
                if (title_node == null) continue;

                var description_node = node.SelectSingleNode(".//p");
                if (description_node == null) continue;

                var list = node.SelectNodes(".//strong");
                if (list == null) continue;

                var parent_asin = parent_asin_node.Attributes["value"].Value;
                var asin = asin_node.Attributes["value"].Value;
                var title = title_node.InnerText;
                var description = description_node.InnerText;
                var authors = list[0].InnerText;
                var narrators = list[1].InnerText;

                if (string.IsNullOrWhiteSpace(parent_asin))
                {
                    Console.WriteLine("[{0}] - {1}", asin, title);
                    Console.WriteLine(authors);
                    Console.WriteLine(narrators);
                    Console.WriteLine(description);
                }
                else
                    Console.WriteLine(" - [{0} - {1}] - {2}", asin, parent_asin, title);


            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
