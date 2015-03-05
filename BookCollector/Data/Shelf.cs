using System.Collections.Generic;

namespace BookCollector.Data
{
    public class Shelf
    {
        public string Name { get; set; }
        public List<Book> Books { get; set; }

        public Shelf()
        {
            Books = new List<Book>();
        }
    }
}
