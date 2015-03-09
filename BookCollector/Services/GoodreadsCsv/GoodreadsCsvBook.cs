using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;

namespace BookCollector.Services.GoodreadsCsv
{
    public class GoodreadsCsvBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string AdditionalAuthors { get; set; }
        public string ISBN { get; set; }
        public string ISBN13 { get; set; }
        public string Bookshelves { get; set; }
        public string ExclusiveShelf { get; set; }

        public string GetShelf()
        {
            if (!string.IsNullOrWhiteSpace(Bookshelves))
                return Bookshelves;
            if (!string.IsNullOrWhiteSpace(ExclusiveShelf))
                return ExclusiveShelf;
            return string.Empty;
        }

        public Book Convert()
        {
            var authors = new List<string> {Author};
            if (!string.IsNullOrWhiteSpace(AdditionalAuthors))
                authors.AddRange(AdditionalAuthors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));

            return new Book
            {
                Title = Title,
                Authors = authors.Distinct().ToList(),
                ISBN10 = ISBN,
                ISBN13 = ISBN13,
            };
        }
    }
}
