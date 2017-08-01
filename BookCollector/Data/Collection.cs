using System.Collections.Generic;
using Core.Utility;
using ReactiveUI;

namespace BookCollector.Data
{
    public class Collection : DirtyTrackingBase
    {
        private Description _Description;
        public Description Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        private Shelf _DefaultShelf;
        public Shelf DefaultShelf
        {
            get { return _DefaultShelf; }
            set { this.RaiseAndSetIfChanged(ref _DefaultShelf, value); }
        }

        private ReactiveList<Shelf> _Shelves = new ReactiveList<Shelf>();
        public ReactiveList<Shelf> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        public List<Book> Books { get { return DefaultShelf.Books; } }
    }
}
