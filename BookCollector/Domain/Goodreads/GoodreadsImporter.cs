using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookCollector.Data;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Mapping;
using CsvHelper.Configuration;

namespace BookCollector.Domain.Goodreads
{
    public class GoodreadsImporter : IImporter
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private List<GoodreadsBook> goodreads_books;
        private List<Book> books;
        private Dictionary<Book, GoodreadsBook> mapping;

        public List<Book> Import(string filename)
        {
            log.Info($"Importing {filename}");

            ParseFile(filename);
            MapBooks();
            UpdateBookHistory();
            HandleShelves();

            log.Info($"Imported {books.Count} books");
            return books;
        }

        private void ParseFile(string filename)
        {
            var configuration = new CsvConfiguration
            {
                IsHeaderCaseSensitive = false,
                IgnoreHeaderWhiteSpace = true,
                TrimFields = true
            };

            using (var sr = new StreamReader(filename))
            using (var csv = new TrimmingCsvReader(sr, configuration))
            {
                goodreads_books = csv.GetRecords<GoodreadsBook>().ToList();
            }
        }

        private void MapBooks()
        {
            mapping = goodreads_books.ToDictionary(b => Mapper.Map<Book>(b));
            books = mapping.Keys.ToList();
        }

        private void UpdateBookHistory()
        {
            books.Apply(b => b.History.Add($"Imported on the {DateTime.Now.ToShortDateString()}"));
        }

        private void HandleShelves()
        {
            // Add shelves to collection
            var shelves = goodreads_books.SelectMany(b => b.Shelves)
                                         .Distinct()
                                         .Select(s => new Shelf(s))
                                         .ToReactiveList();

            // Update the shelves for each book
            books.Apply(b => 
            {
                var goodreads_book = mapping[b];

                var shelves_for_current_book = shelves.Where(s => goodreads_book.Shelves.Contains(s.Name));
                shelves_for_current_book.Apply(s => s.Add(b));
            });
        }
    }
}
