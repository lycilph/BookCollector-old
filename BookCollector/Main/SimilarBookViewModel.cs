using BookCollector.Data;
using Framework.Mvvm;

namespace BookCollector.Main
{
    public class SimilarBookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }
        public string Image { get { return AssociatedObject.Image; } }

        public SimilarBookViewModel(Book book) : base(book) { }
    }
}
