using System;
using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Data
{
    public class ShelfViewModel : ItemViewModel<Shelf>
    {
        public string Name
        {
            get { return Obj.Name; }
            set
            {
                if (value != Obj.Name)
                {
                    this.RaisePropertyChanging();
                    Obj.Name = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsLocked { get { return Obj.IsLocked; } }

        private int _BooksCount;
        public int BooksCount
        {
            get { return _BooksCount; }
            set { this.RaiseAndSetIfChanged(ref _BooksCount, value); }
        }

        public ShelfViewModel(Shelf obj) : base(obj)
        {
            this.WhenAnyValue(x => x.Obj.Books.Count)
                .Subscribe(count => BooksCount = count);
        }
    }
}
