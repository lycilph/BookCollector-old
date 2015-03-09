using System.Collections.Generic;

namespace BookCollector.Data
{
    public class Collection
    {
        public string Name { get; set; }
        public List<Shelf> Shelfs { get; set; }
        public List<Book> Books { get; set; }

        public Collection()
        {
            Shelfs = new List<Shelf>();
            Books = new List<Book>();
        }
    }
}
