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

        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { this.RaiseAndSetIfChanged(ref _IsChecked, value); }
        }

        private int _BooksCount;
        public int BooksCount
        {
            get { return _BooksCount; }
            set { this.RaiseAndSetIfChanged(ref _BooksCount, value); }
        }

        public bool Locked { get { return obj.Locked; } }

        public ShelfViewModel(Shelf obj) : base(obj) { }
    }
}
