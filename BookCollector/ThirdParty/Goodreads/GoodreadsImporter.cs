using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookCollector.Data.Import;
using Core.Mapping;
using CsvHelper.Configuration;

namespace BookCollector.ThirdParty.Goodreads
{
    public static class GoodreadsImporter
    {
        public static List<ImportedBook> Import(string filename)
        {
            var configuration = new CsvConfiguration
            {
                IsHeaderCaseSensitive = false,
                IgnoreHeaderWhiteSpace = true,
                TrimFields = true
            };

            List<GoodreadsBook> goodreads_books;

            using (var sr = new StreamReader(filename))
            using (var csv = new TrimmingCsvReader(sr, configuration))
            {
                goodreads_books = csv.GetRecords<GoodreadsBook>().ToList();
            }

            return goodreads_books.Select(b => Mapper.Map<ImportedBook>(b)).ToList();
        }
    }
}
