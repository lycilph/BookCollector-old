using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookCollector.Data;
using Caliburn.Micro;
using CsvHelper;
using CsvHelper.Configuration;

namespace BookCollector.Services.GoodreadsCsv
{
    public class Importer
    {
        private class CustomCsvReader : CsvReader
        {
            public CustomCsvReader(TextReader reader, CsvConfiguration configuration) : base(reader, configuration) { }

            public override string GetField(int index)
            {
                var field = base.GetField(index);
                return field.TrimStart('=').TrimStart('=').Replace("\"", "");
            }
        }

        public static List<Book> Read(string filename)
        {
            var configuration = new CsvConfiguration
            {
                IsHeaderCaseSensitive = false,
                IgnoreHeaderWhiteSpace = true,
                TrimFields = true
            };

            using (var sr = new StreamReader(filename))
            using (var csv = new CustomCsvReader(sr, configuration))
            {
                var csv_books = csv.GetRecords<GoodreadsCsvBook>().ToList();
                var books = csv_books.Select(b => b.Convert()).ToList();

                books.Apply(b =>
                {
                    b.Source = "Goodreads CSV";
                    b.History.Add("Imported on " + DateTime.Now.ToShortDateString());
                });

                //var shelves = csv_books.Select(b => b.GetShelf()).Distinct().ToList();

                return books;
            }
        }
    }
}
