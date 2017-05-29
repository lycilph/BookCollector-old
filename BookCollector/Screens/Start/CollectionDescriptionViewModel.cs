using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Start
{
    public class CollectionDescriptionViewModel : ItemViewModel<CollectionDescription>
    {
        public string Name
        {
            get { return obj.Name; }
            set
            {
                if (value != obj.Name)
                {
                    this.RaisePropertyChanging();
                    obj.Name = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Description
        {
            get { return obj.Description; }
        }

        public CollectionDescriptionViewModel(CollectionDescription description) : base(description) { }
    }
}
