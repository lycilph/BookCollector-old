using System;
using System.Linq;
using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Data
{
    public class BookViewModel : ItemViewModel<Book>
    {
        public string Title { get { return Obj.Title; } }
        public string Authors { get { return string.Join(", ", Obj.Authors); } }
        public string ISBN10 { get { return Obj.ISBN10; } }
        public string ISBN13 { get { return Obj.ISBN13; } }

        private string _ShelvesAsText;
        public string ShelvesAsText
        {
            get { return _ShelvesAsText; }
            set { this.RaiseAndSetIfChanged(ref _ShelvesAsText, value); }
        }

        public BookViewModel(Book obj) : base(obj)
        {
            this.WhenAnyValue(x => x.Obj.Shelves.Count)
                .Subscribe(count => ShelvesAsText = string.Join(", ", Obj.Shelves.Select(s => s.Name).OrderBy(s => s)));
        }

        public bool IsOnShelf(Shelf shelf)
        {
            return Obj.IsOnShelf(shelf);
        }
    }
}
