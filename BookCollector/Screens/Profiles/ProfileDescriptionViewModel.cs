using BookCollector.Model;
using Framework.Core.MVVM;
using ReactiveUI;
using System.Linq;

namespace BookCollector.Screens.Profiles
{
    public class ProfileDescriptionViewModel : ItemViewModelBase<ProfileDescription>
    {
        public string DisplayName
        {
            get { return AssociatedObject.DisplayName; }
            set { AssociatedObject.DisplayName = value; }
        }

        private IReactiveDerivedList<CollectionDescriptionViewModel> _Collections;
        public IReactiveDerivedList<CollectionDescriptionViewModel> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        private CollectionDescriptionViewModel _CurrentCollection;
        public CollectionDescriptionViewModel CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        public ProfileDescriptionViewModel(ProfileDescription obj) : base(obj)
        {
            _Collections = AssociatedObject.Collections.CreateDerivedCollection(c => new CollectionDescriptionViewModel(c));
            CurrentCollection = Collections.FirstOrDefault();
        }
    }
}
