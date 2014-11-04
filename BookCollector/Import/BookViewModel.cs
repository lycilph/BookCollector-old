using BookCollector.Model;
using Framework.Core.MVVM;

namespace BookCollector.Import
{
    public class BookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public BookViewModel(Book obj) : base(obj) { }
    }
}
