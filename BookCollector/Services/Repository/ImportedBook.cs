using System.Collections.Generic;
using BookCollector.Model;

namespace BookCollector.Services.Repository
{
    public class ImportedBook
    {
        public Book Book { get; set; }
        public List<ImageLink> ImageLinks { get; set; }
    }
}
