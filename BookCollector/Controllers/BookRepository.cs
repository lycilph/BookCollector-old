using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using BookCollector.Model;
using BookCollector.Services;
using BookCollector.Utilities;
using NLog;
using ReactiveUI;

namespace BookCollector.Controllers
{
    [Export(typeof(BookRepository))]
    public class BookRepository : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string filename = "collection.txt";

        private readonly ApplicationSettings application_settings;
        private Dictionary<string, Book> id_to_books;

        private List<Book> _Books = new List<Book>();
        public List<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        [ImportingConstructor]
        public BookRepository(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;
        }

        private string GetPath(CollectionDescription collection)
        {
            var dir = Path.Combine(application_settings.DataDir, collection.Id);
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, filename);
        }

        public void Clear()
        {
            Books.Clear();
        }

        public void Add(IEnumerable<Book> books)
        {
            Books.AddRange(books);
        }

        public void Load(CollectionDescription collection)
        {
            var path = GetPath(collection);
            if (!File.Exists(path))
            {
                logger.Trace("No collection found");
                Books = new List<Book>();
                id_to_books = new Dictionary<string, Book>();
                return;
            }

            logger.Trace("Loading (path = {0})", path);
            Books = JsonExtensions.DeserializeFromFile<List<Book>>(path);
            id_to_books = Books.ToDictionary(b => b.Id);
        }


        public void Save(CollectionDescription collection)
        {
            var path = GetPath(collection);
            logger.Trace("Saving (path = {0})", path);
            JsonExtensions.SerializeToFile(path, Books);
        }

        public Book GetDuplicate(Book book)
        {
            return Books.FirstOrDefault(book.IsDuplicate);
        }

        public Book Get(string book_id)
        {
            return id_to_books.ContainsKey(book_id) ? id_to_books[book_id] : null;
        }
    }
}
