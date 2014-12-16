using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using BookCollector.Model;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;

namespace BookCollector.Services
{
    [Export(typeof(BookRepository))]
    public class BookRepository : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private List<Book> _Books = new List<Book>();
        public List<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private static string GetFilename()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(folder, "Books.txt");
        }

        public void AddRange(IEnumerable<Book> books)
        {
            Books.AddRange(books);
        }

        public void Clear()
        {
            Books.Clear();
        }

        public void Load()
        {
            logger.Trace("Loading");

            var path = GetFilename();
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            Books = JsonConvert.DeserializeObject<List<Book>>(json);
        }

        public void Save()
        {
            logger.Trace("Saving");

            var path = GetFilename();
            var json = JsonConvert.SerializeObject(Books, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public bool IsDuplicate(Book book)
        {
            return Books.Any(book.IsDuplicate);
        }

        public Book Get(string book_id)
        {
            return Books.SingleOrDefault(b => b.Id == book_id);
        }
    }
}
