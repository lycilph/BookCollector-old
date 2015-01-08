using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using BookCollector.Services.Settings;
using BookCollector.Utilities;
using NLog;
using ReactiveUI;

namespace BookCollector.Model
{
    [Export(typeof(BookRepository))]
    public class BookRepository : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ApplicationSettings application_settings;

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
            var path = Path.Combine(application_settings.DataDir, collection.Id + "_collection.txt");
            if (File.Exists(path))
            {
                logger.Trace("Loading collection " + collection);
                Books = JsonExtensions.DeserializeFromFile<List<Book>>(path);
            }
            else
            {
                logger.Trace("No collection found for " + collection);
                Books = new List<Book>();
            }
        }

        public void Save(CollectionDescription collection)
        {
            logger.Trace("Saving collection " + collection);
            var path = Path.Combine(application_settings.DataDir, collection.Id + "_collection.txt");
            JsonExtensions.SerializeToFile(path, Books);
        }

        public Book GetDuplicate(Book book)
        {
            return Books.FirstOrDefault(book.IsDuplicate);
        }

        public Book Get(string book_id)
        {
            return Books.SingleOrDefault(b => b.Id == book_id);
        }
    }
}
