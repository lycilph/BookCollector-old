using BookCollector.Data;
using Panda.ApplicationCore.Utilities;

namespace BookCollector.Screens.Selection
{
    public class CollectionViewModel : ItemViewModelBase<Collection>
    {
        public string Name
        {
            get { return AssociatedObject.Name; }
            set { AssociatedObject.Name = value; }
        }

        public CollectionViewModel(Collection obj) : base(obj) { }
    }
}
