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

        private ReactiveList<CollectionViewModel> _Collections;
        public ReactiveList<CollectionViewModel> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        public UserViewModel(User user) : base(user)
        {
            Collections = AssociatedObject.Collections.Select(c => new CollectionViewModel(c)).ToReactiveList();

            if (Collections != null && Collections.Any())
                CurrentCollection = Collections.First();
        }
    }
}
