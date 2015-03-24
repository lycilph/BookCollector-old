using System.Linq;
using BookCollector.Data;
using Panda.ApplicationCore.Utilities;
using ReactiveUI;

namespace BookCollector.Screens.Selection
{
    public class UserViewModel : ItemViewModelBase<User>
    {
        public string Name
        {
            get { return AssociatedObject.Name; }
            set { AssociatedObject.Name = value; }
        }

        private CollectionViewModel _CurrentCollection;
        public CollectionViewModel CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        private IReactiveDerivedList<CollectionViewModel> _Collections;
        public IReactiveDerivedList<CollectionViewModel> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        private bool _IsEditing;
        public bool IsEditing
        {
            get { return _IsEditing; }
            set { this.RaiseAndSetIfChanged(ref _IsEditing, value); }
        }

        public UserViewModel(User user) : base(user)
        {
            Collections = AssociatedObject.Collections.CreateDerivedCollection(c => new CollectionViewModel(c));

            if (Collections != null && Collections.Any())
                CurrentCollection = Collections.First();
        }
    }
}
