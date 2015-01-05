using System.Collections.Generic;

namespace BookCollector.Model
{
    public class ImportedBook
    {
        public Book Book { get; set; }
        public List<ImageLink> ImageLinks { get; set; }
    }
}
