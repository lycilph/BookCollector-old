using BookCollector.Data;
using Panda.ApplicationCore.Utilities;
using ReactiveUI;

namespace BookCollector.Screens.Selection
{
    public class CollectionViewModel : ItemViewModelBase<Collection>
    {
        public string Name
        {
            get { return AssociatedObject.Name; }
            set { AssociatedObject.Name = value; }
        }

        private bool _IsEditing;
        public bool IsEditing
        {
            get { return _IsEditing; }
            set { this.RaiseAndSetIfChanged(ref _IsEditing, value); }
        }

        public CollectionViewModel(Collection obj) : base(obj) { }
    }
}
