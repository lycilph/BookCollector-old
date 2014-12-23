using BookCollector.Services.Books;
using Framework.Core.MVVM;

namespace BookCollector.Screens.Main
{
    public class MainBookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public string Description { get { return AssociatedObject.Description; } }

        public string Image { get { return AssociatedObject.Image; } }

        public MainBookViewModel(Book obj) : base(obj)
        {
        }
    }
}
