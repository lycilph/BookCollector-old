using BookCollector.Data;
using Framework.Mvvm;

namespace BookCollector.Main
{
    public class BookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }
        public string Author { get { return AssociatedObject.Author; } }
        public string Image { get { return AssociatedObject.Image; } }

        public BookViewModel(Book book) : base(book)
        {
        }
    }
}
