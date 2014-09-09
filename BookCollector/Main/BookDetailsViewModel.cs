using System;
using BookCollector.Data;
using Framework.Dialogs;
using Framework.Mvvm;

namespace BookCollector.Main
{
    public class BookDetailsViewModel : ItemViewModelBase<Book>, IHaveCloseAction
    {
        public Action CloseCallback { get; set; }

        public BookDetailsViewModel(Book book) : base(book) { }
    }
}
