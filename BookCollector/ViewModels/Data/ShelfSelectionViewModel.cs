using System;
using System.Reactive.Linq;
using BookCollector.Data;
using ReactiveUI;

namespace BookCollector.ViewModels.Data
{
    public class ShelfSelectionViewModel : ReactiveObject
    {
        public Shelf Shelf { get; set; }

        private Book _Book;
        public Book Book
        {
            get { return _Book; }
            set { this.RaiseAndSetIfChanged(ref _Book, value); }
        }

        private bool _IsChecked = false;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { this.RaiseAndSetIfChanged(ref _IsChecked, value); }
        }

        public ShelfSelectionViewModel(Book book, Shelf shelf)
        {
            Book = book;
            Shelf = shelf;

            this.WhenAnyValue(x => x.Book)
                .Where(b => b != null)
                .Subscribe(_ => IsChecked = Book.IsOnShelf(Shelf));

            this.WhenAnyValue(x => x.IsChecked)
                .Where(_ => Book != null)
                .Subscribe(value => UpdateShelf(value));
        }

        private void UpdateShelf(bool value)
        {
            if (value)
                Shelf.Add(Book);
            else
                Shelf.Remove(Book);
        }
    }
}
