﻿using BookCollector.Data;
using ReactiveUI;

namespace BookCollector.Screens.Main
{
    public class BookViewModel : ReactiveObject
    {
        private Book book;

        public string Title { get { return book.Title; } }
        public string Authors { get { return string.Join(", ", book.Authors); } }
        public string ISBN10 { get { return book.ISBN10; } }
        public string ISBN13 { get { return book.ISBN13; } }

        public BookViewModel(Book book)
        {
            this.book = book;
        }
    }
}
