using System.Collections.Generic;

namespace Tests
{
    public class Book
    {
        public string Title { get; set; }
        public List<Shelf> Shelves { get; set; }

        public Book(string title)
        {
            Title = title;
        }
    }
}
