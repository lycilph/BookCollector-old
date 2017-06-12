using System.Collections.Generic;
using BookCollector.Data;
using BookCollector.Models;

namespace BookCollector.Screens.Books
{
    public class BookViewModel
    {
        private Book book;

        public string Title { get { return book.Title; } }
        public string Authors { get { return string.Join(", ", book.Authors); } }
        public string ISBN10 { get { return book.ISBN10; } }
        public string ISBN13 { get { return book.ISBN13; } }
        public List<Shelf> Shelves { get { return book.Shelves; } }

        public BookViewModel(Book book)
        {
            this.book = book;
        }
    }
}
