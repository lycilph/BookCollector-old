using BookCollector.Model;

namespace BookCollector.Services.Import
{
    public class ImportedBook
    {
        public Book Book { get; set; }
        public ImageLinks ImageLinks { get; set; }
    }
}
