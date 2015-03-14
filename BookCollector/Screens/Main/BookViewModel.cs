using BookCollector.Data;
using Panda.ApplicationCore.Utilities;

namespace BookCollector.Screens.Main
{
    public class BookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }
        public string Authors { get { return string.Join(", ", AssociatedObject.Authors); } }
        public string ISBN10 { get { return AssociatedObject.ISBN10; } }
        public string ISBN13 { get { return AssociatedObject.ISBN13; } }

        public BookViewModel(Book obj) : base(obj) { }
    }
}
