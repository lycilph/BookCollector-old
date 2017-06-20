using System.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.ViewModels.Common;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class BooksScreenViewModel : ScreenBase
    {
        private IApplicationModel application_model;
        private IEventAggregator event_aggregator;
        private ISnackbarMessageQueue message_queue;

        private ReactiveList<ShelfViewModel> _Shelves;
        public ReactiveList<ShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private ShelfViewModel _SelectedShelf;
        public ShelfViewModel SelectedShelf
        {
            get { return _SelectedShelf; }
            set { this.RaiseAndSetIfChanged(ref _SelectedShelf, value); }
        }

        private ReactiveList<BookViewModel> _Books;
        public ReactiveList<BookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private BookViewModel _SelectedBook;
        public BookViewModel SelectedBook
        {
            get { return _SelectedBook; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBook, value); }
        }

        public BooksScreenViewModel(IApplicationModel application_model, IEventAggregator event_aggregator, ISnackbarMessageQueue message_queue)
        {
            this.application_model = application_model;
            this.event_aggregator = event_aggregator;
            this.message_queue = message_queue;

            DisplayName = Constants.BooksScreenDisplayName;
        }

        public override void Activate()
        {
            base.Activate();

            Books = application_model.CurrentCollection.Books.Select(b => new BookViewModel(b)).ToReactiveList();
            SelectedBook = Books.FirstOrDefault();

            Shelves = application_model.CurrentCollection.Shelves.Select(s => new ShelfViewModel(s)).ToReactiveList();
            SelectedShelf = Shelves.FirstOrDefault();
            
            if (!Books.Any())
                message_queue.Enqueue("No Books?", "Import Here", () => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.ImportScreenDisplayName)));
        }
    }
}
