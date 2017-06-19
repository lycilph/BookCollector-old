using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookCollector.Data;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Mapping;
using BookCollector.ViewModels.Common;
using CsvHelper.Configuration;
using ReactiveUI;

namespace BookCollector.Domain.Goodreads
{
    public class GoodreadsImporter
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private Collection collection;

        private Dictionary<Book, GoodreadsBook> mapping;
        private List<GoodreadsBook> goodreads_books;

        private List<Book> books;
        public List<Book> ImportedBooks { get { return books; } }

        private List<Shelf> shelves;
        public List<Shelf> ImportedShelves { get { return shelves; } }

        public GoodreadsImporter(string filename, Collection collection)
        {
            log.Info($"Importing {filename}");

            this.collection = collection;

            ParseFile(filename);
            MapBooks();
            HandleShelves();

            log.Info($"Imported {books.Count} books");
        }

        public ReactiveList<ImportedBookViewModel> GetViewModels()
        {
            var view_models = books.Select(b => new ImportedBookViewModel(b)).ToReactiveList();
            CalculateSimilarity(view_models);
            return view_models;
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

        private void HandleShelves()
        {
            var shelves_found = goodreads_books.SelectMany(b => b.Shelves)
                                               .Distinct()
                                               .ToList();

            // shelves = new shelves found minus existing shelves
            shelves = shelves_found.Where(name => !collection.Shelves.Any(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                                   .Select(name => new Shelf(name))
                                   .ToList();

            // Update the shelves for each book
            var all_shelf = collection.Shelves.Single(s => s.Name == Constants.AllShelfName);
            books.Apply(b =>
            {
                var goodreads_book = mapping[b];
                var shelf_names = goodreads_book.Shelves;
                var shelves_for_book = shelves.Where(s => shelf_names.Contains(s.Name)).ToList();
                shelves_for_book.Add(all_shelf);

                b.Shelves.AddRange(shelves_for_book);
            });
        }

        private void CalculateSimilarity(ReactiveList<ImportedBookViewModel> view_models)
        {
            var books_in_collection = collection.Books;

            foreach (var vm in view_models)
            {
                var vm_book = vm.Unwrap();
                var duplicates = books_in_collection.Where(b => vm_book.Title.Equals(b.Title) ||
                                                                vm_book.Authors.SequenceEqual(b.Authors) ||
                                                                vm_book.ISBN10.Equals(b.ISBN10) ||
                                                                vm_book.ISBN13.Equals(b.ISBN13))
                                                    .Select(b =>
                                                    {
                                                        var score = 0;
                                                        var text_short = string.Empty;
                                                        var text = string.Empty;
                                                        if (vm_book.Title.Equals(b.Title))
                                                        {
                                                            score += 25;
                                                            text_short = "T";
                                                            text = "Title";
                                                        }
                                                        if (vm_book.Authors.SequenceEqual(b.Authors))
                                                        {
                                                            score += 25;
                                                            text_short += ", A";
                                                            text += ", Authors";
                                                        }
                                                        if (!string.IsNullOrWhiteSpace(vm_book.ISBN10) && vm_book.ISBN10.Equals(b.ISBN10))
                                                        {
                                                            score += 25;
                                                            text_short += ", 10";
                                                            text += ", ISBN10";
                                                        }
                                                        if (!string.IsNullOrWhiteSpace(vm_book.ISBN13) && vm_book.ISBN13.Equals(b.ISBN13))
                                                        {
                                                            score += 25;
                                                            text_short += ", 13";
                                                            text += ", ISBN13";
                                                        }

                                                        text_short = text_short.TrimStart(new char[] { ',', ' ' });
                                                        text = text.TrimStart(new char[] { ',', ' ' });

                                                        return new { b, score, text_short, text };
                                                    })
                                                    .OrderByDescending(p => p.score);

                if (duplicates.Any())
                {
                    vm.Similarity = duplicates.First().score;
                    vm.SimilarityTextShort = duplicates.First().text_short;
                    vm.SimilarityText = duplicates.First().text;
                }
            }
        }
    }
}
