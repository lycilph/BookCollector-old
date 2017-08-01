using System.Collections.Generic;
using Core.Extensions;

namespace BookCollector.Data
{
    public class Shelf
    {
        public List<Book> Books { get; set; } = new List<Book>();

        public void Add(List<Book> books_to_add)
        {
            books_to_add.Apply(Add);
        }

        public void Add(Book book)
        {
            // Add to list of books on shelf
            Books.Add(book);

            // Add a link from the book to this shelf
            book.Shelves.Add(this);
        }
    }
}
