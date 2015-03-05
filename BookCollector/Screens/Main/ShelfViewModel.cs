using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;
using Panda.ApplicationCore.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Main
{
    public class ShelfViewModel : ItemViewModelBase<Shelf>
    {
        public string DisplayName { get { return AssociatedObject.Name; } }
        public List<BookViewModel> Books { get; set; }

        private BookViewModel _SelectedBook;
        public BookViewModel SelectedBook
        {
            get { return _SelectedBook; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBook, value); }
        }

        public ShelfViewModel(Shelf obj) : base(obj)
        {
            Books = obj.Books.Select(b => new BookViewModel(b)).ToList();
        }
    }
}
