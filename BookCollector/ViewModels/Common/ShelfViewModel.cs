using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Common
{
    public class ShelfViewModel : ItemViewModel<Shelf>
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

        public int BooksCount { get; set; }
        public bool Locked { get { return obj.Locked; } }

        public ShelfViewModel(Shelf obj) : base(obj) { }
    }
}
