using System;
using System.Linq;
using BookCollector.Data;
using BookCollector.Domain;
using BookCollector.Framework.Messaging;
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

        private ReactiveCommand _SearchOnWebCommand;
        public ReactiveCommand SearchOnWebCommand
        {
            get { return _SearchOnWebCommand; }
            set { this.RaiseAndSetIfChanged(ref _SearchOnWebCommand, value); }
        }

        public BookViewModel(Book obj, IEventAggregator event_aggregator) : base(obj)
        {
            this.WhenAnyValue(x => x.Obj.Shelves.Count)
                .Subscribe(count => ShelvesAsText = string.Join(", ", Obj.Shelves.Select(s => s.Name).OrderBy(s => s)));

            SearchOnWebCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.SearchOnWeb(Title)));
        }

        public bool IsOnShelf(Shelf shelf)
        {
            return Obj.IsOnShelf(shelf);
        }
    }
}
