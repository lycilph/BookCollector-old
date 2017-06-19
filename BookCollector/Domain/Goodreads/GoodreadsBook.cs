using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Framework.Extensions;

namespace BookCollector.Domain.Goodreads
{
    public class GoodreadsBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string AdditionalAuthors { get; set; }
        public string ISBN { get; set; }
        public string ISBN13 { get; set; }
        public string Bookshelves { get; set; }
        public string ExclusiveShelf { get; set; }
        public List<string> Shelves
        {
            get
            {
                return Bookshelves.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(a => a.Trim())
                                  .AddTo(ExclusiveShelf)
                                  .Distinct()
                                  .ToList();
            }
        }
        public string Source { get { return "Goodreads CSV"; } }
    }
}
