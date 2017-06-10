using System.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class BooksViewModel : MainScreenBase
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IApplicationModel application_model;

        private IReactiveDerivedList<BookViewModel> _Books;
        public IReactiveDerivedList<BookViewModel> Books
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

        private ReactiveCommand _WebCommand;
        public ReactiveCommand WebCommand
        {
            get { return _WebCommand; }
            set { this.RaiseAndSetIfChanged(ref _WebCommand, value); }
        }

        private ReactiveCommand _ImportCommand;
        public ReactiveCommand ImportCommand
        {
            get { return _ImportCommand; }
            set { this.RaiseAndSetIfChanged(ref _ImportCommand, value); }
        }

        private ReactiveCommand _ChangeCollectionCommand;
        public ReactiveCommand ChangeCollectionCommand
        {
            get { return _ChangeCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _ChangeCollectionCommand, value); }
        }

        public BooksViewModel(IEventAggregator event_aggregator, IApplicationModel application_model, SearchViewModel search_view_model, ShelvesViewModel menu_view_model)
        {
            this.application_model = application_model;

            DisplayName = ScreenNames.BooksName;
            ShowCollectionCommand = true;
            ExtraContent = search_view_model;
            MenuContent = menu_view_model;

            WebCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.WebName)));
            ChangeCollectionCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.CollectionsName)));
            ImportCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.ImportName)));
        }

        public override void Activate()
        {
            Books = application_model.CurrentCollection.Books.CreateDerivedCollection(b => new BookViewModel(b));
            SelectedBook = Books.FirstOrDefault();
        }

        public override void Deactivate()
        {
            application_model.SaveCurrentCollection();
        }
    }
}
