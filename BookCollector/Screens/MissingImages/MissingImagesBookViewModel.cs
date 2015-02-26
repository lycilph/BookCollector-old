using BookCollector.Model;
using Framework.Core.MVVM;

namespace BookCollector.Screens.MissingImages
{
    public class MissingImagesBookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public string Authors { get { return string.Join(",", AssociatedObject.Authors); } }

        public string Image { get { return AssociatedObject.Image; } }

        public MissingImagesBookViewModel(Book obj) : base(obj) { }
    }
}
