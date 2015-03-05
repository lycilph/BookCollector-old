using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookCollector.Data;
using Caliburn.Micro;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace BookCollector.Services
{
    public class Importer
    {
        private class CustomCsvReader : CsvReader
        {
            public CustomCsvReader(TextReader reader) : base(reader) { }
            public CustomCsvReader(TextReader reader, CsvConfiguration configuration) : base(reader, configuration) { }
            public CustomCsvReader(ICsvParser parser) : base(parser) { }

            public override string GetField(int index)
            {
                var field = base.GetField(index);
                return field.TrimStart('=').TrimStart('=').Replace("\"", "");
            }
        }

        private sealed class BookMap : CsvClassMap<Book>
        {
            public BookMap()
            {
                Map(m => m.Title);
                Map(m => m.Authors).ConvertUsing(GetAuthors);
                Map(m => m.ISBN10).Name("isbn");
                Map(m => m.ISBN13);
            }

            private static object GetAuthors(ICsvReaderRow row)
            {
                var result = new List<string> {row.GetField("Author")};

                var additional_authors = row.GetField("Additional Authors");
                if (string.IsNullOrWhiteSpace(additional_authors)) 
                    return result;

                var authors = additional_authors.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim());
                result.AddRange(authors);
                return result.Distinct().ToList();
            }
        }

        public static List<Book> Read(string filename)
        {
            var configuration = new CsvConfiguration
            {
                IsHeaderCaseSensitive = false,
                TrimFields = true
            };
            configuration.RegisterClassMap(new BookMap());

            using (var sr = new StreamReader(filename))
            using (var csv = new CustomCsvReader(sr, configuration))
            {
                var books = csv.GetRecords<Book>().ToList();
                books.Apply(b => b.ImportSource = "Goodreads CSV");
                return books;
            }
        }
    }
}
