using System.Linq;
using BookCollector.Data;
using Panda.ApplicationCore.Extensions;
using Panda.ApplicationCore.MVVM;
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

        public ReactiveList<BookViewModel> Books { get; set; }

        public ShelfViewModel(Shelf obj) : base(obj)
        {
            Books = obj.Books.Select(b => new BookViewModel(b)).ToReactiveList();
        }

        public void Add(BookViewModel book_view_model)
        {
            AssociatedObject.Books.Add(book_view_model.AssociatedObject);
            Books.Add(book_view_model);
        }

        public void Remove(BookViewModel book_view_model)
        {
            AssociatedObject.Books.Remove(book_view_model.AssociatedObject);
            Books.Remove(book_view_model);
        }
    }
}
