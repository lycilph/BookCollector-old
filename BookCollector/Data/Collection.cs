using System.Collections.Generic;
using BookCollector.Framework.MVVM;

namespace BookCollector.Data
{
    public class Collection : DirtyTrackingBase
    {
        public Description Description { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
        public List<Shelf> Shelves { get; set; } = new List<Shelf>();
        public Book SelectedBook { get; set; }
        public Shelf SelectedShelf { get; set; }
    }
}
