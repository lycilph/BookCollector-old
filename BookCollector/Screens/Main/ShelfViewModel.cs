using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;
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

        public List<BookViewModel> Books { get; set; }

        public ShelfViewModel(Shelf obj) : base(obj)
        {
            Books = obj.Books.Select(b => new BookViewModel(b)).ToList();
        }
    }
}
