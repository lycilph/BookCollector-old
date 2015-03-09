using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Data;
using BookCollector.Services.GoodreadsCsv;

namespace BookCollector.Controllers
{
    [Export(typeof(IDataController))]
    public class DataController : IDataController
    {
        public User User { get; set; }
        public Collection Collection { get; set; }

        public DataController()
        {
            User = new User();
            Collection = new Collection();
        }

        public void Initialize()
        {
            const string csv_filename = @"C:\Private\Projects\BookCollector\Data\goodreads_export.csv";
            var books = Importer.Read(csv_filename).ToList();

            Collection.Books.AddRange(books);
            Collection.Shelfs.Add(new Shelf
            {
                Name = "All",
                Books = books
            });

            User.Name = "[Name]";
            User.Collections.Add(Collection);
        }
    }
}
