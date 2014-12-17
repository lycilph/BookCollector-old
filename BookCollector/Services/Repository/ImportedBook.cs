using BookCollector.Model;

namespace BookCollector.Services.Repository
{
    public class ImportedBook
    {
        public Book Book { get; set; }
        public ImageLinks ImageLinks { get; set; }
    }
}
