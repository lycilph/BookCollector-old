using BookCollector.Services.Collections;
using Framework.Core.MVVM;

namespace BookCollector.Screens.Collections
{
    public class CollectionDescriptionViewModel : ItemViewModelBase<CollectionDescription>
    {
        public string DisplayName 
        {
            get { return AssociatedObject.DisplayName; }
            set { AssociatedObject.DisplayName = value; }
        }

        public string Info
        {
            get { return AssociatedObject.Info; }
        }

        public string Dates
        {
            get
            {
                var created = AssociatedObject.Created.ToShortDateString();
                var modified = AssociatedObject.LastModified.ToShortDateString();
                return string.Format("Created: {0}, Last modified: {1}", created, modified);
            }
        }

        public CollectionDescriptionViewModel(CollectionDescription obj) : base(obj)
        {
        }
    }
}
