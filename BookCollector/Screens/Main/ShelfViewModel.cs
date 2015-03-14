using BookCollector.Data;
using Panda.ApplicationCore.Utilities;
using ReactiveUI;

namespace BookCollector.Screens.Main
{
    public class ShelfViewModel : ItemViewModelBase<Shelf>
    {
        public string DisplayName
        {
            get { return AssociatedObject.Name; }
            set { AssociatedObject.Name = value; }
        }

        private BookViewModel _SelectedBook;
        public BookViewModel SelectedBook
        {
            get { return _SelectedBook; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBook, value); }
        }

        public IReactiveDerivedList<BookViewModel> Books { get; set; }

        public ShelfViewModel(Shelf obj) : base(obj)
        {
            Books = obj.Books.CreateDerivedCollection(b => new BookViewModel(b));
        }

        public void Add(BookViewModel book_view_model)
        {
            AssociatedObject.Books.Add(book_view_model.AssociatedObject);
        }

        public void Remove(BookViewModel book_view_model)
        {
            AssociatedObject.Books.Remove(book_view_model.AssociatedObject);
        }
    }
}
