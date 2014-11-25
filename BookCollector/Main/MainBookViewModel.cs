using BookCollector.Model;
using Framework.Core.MVVM;

namespace BookCollector.Main
{
    public class MainBookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public string Description { get { return AssociatedObject.Description; } }

        public MainBookViewModel(Book obj) : base(obj)
        {
        }
    }
}
