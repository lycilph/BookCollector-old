using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Common
{
    public class BookViewModel : ItemViewModel<Book>
    {
        public string Title { get { return obj.Title; } }
        public string Authors { get { return string.Join(", ", obj.Authors); } }
        public string ISBN10 { get { return obj.ISBN10; } }
        public string ISBN13 { get { return obj.ISBN13; } }
        public List<Shelf> Shelves { get { return obj.Shelves; } }

        private string _ShelvesAsText;
        public string ShelvesAsText
        {
            get { return _ShelvesAsText; }
            set { this.RaiseAndSetIfChanged(ref _ShelvesAsText, value); }
        }

        public BookViewModel(Book obj) : base(obj) { }
    }
}
