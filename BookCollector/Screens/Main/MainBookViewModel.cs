using BookCollector.Model;
using Framework.Core.MVVM;

namespace BookCollector.Screens.Main
{
    public class MainBookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public string Authors { get { return string.Join(",", AssociatedObject.Authors); } }

        public string Description { get { return AssociatedObject.Description; } }

        public string Image { get { return AssociatedObject.Image; } }

        public string SmallImage { get { return AssociatedObject.SmallImage; } }

        public string Source { get { return AssociatedObject.ImportSource; } }

        public MainBookViewModel(Book obj) : base(obj) { }
    }
}
