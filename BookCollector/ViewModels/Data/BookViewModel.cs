using BookCollector.Data;
using BookCollector.Framework.MVVM;

namespace BookCollector.ViewModels.Data
{
    public class BookViewModel : ItemViewModel<Book>
    {
        public BookViewModel(Book obj) : base(obj) { }
    }
}
