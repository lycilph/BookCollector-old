using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
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
        private ShelvesViewModel shelves_view_model;
        private List<BookViewModel> all_books;

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

        private ShelfViewModel _Shelf;
        public ShelfViewModel Shelf
        {
            get { return _Shelf; }
            set { this.RaiseAndSetIfChanged(ref _Shelf, value); }
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

        public BooksViewModel(IEventAggregator event_aggregator, IApplicationModel application_model, SearchViewModel search_view_model, ShelvesViewModel shelves_view_model)
        {
            this.application_model = application_model;
            this.shelves_view_model = shelves_view_model;

            DisplayName = ScreenNames.BooksName;
            ShowCollectionCommand = true;
            ExtraContent = search_view_model;
            MenuContent = shelves_view_model;

            this.WhenAnyValue(x => x.application_model.CurrentShelf)
                .Subscribe(_ => UpdateShelf());

            WebCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.WebName)));
            ChangeCollectionCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.CollectionsName)));
            ImportCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.ImportName)));
        }

        public override void Activate()
        {
            all_books = application_model.CurrentCollection.Books.Select(b => new BookViewModel(b)).ToList();
            //Books = all_books.Where(b => b.Shelves.Contains(application_model.CurrentShelf)).ToReactiveList();
            //SelectedBook = Books.FirstOrDefault();

            //Shelf = new ShelfViewModel(application_model.CurrentShelf);

            shelves_view_model.Shelves = application_model.CurrentCollection.Shelves.Select(s => new ShelfViewModel(s))
                                                                                    .OrderBy(s => s.Name)
                                                                                    .ToReactiveList();

            UpdateShelf();
        }

        public override void Deactivate()
        {
            application_model.SaveCurrentCollection();
        }

        private void UpdateShelf()
        {
            if (all_books == null)
                return;

            Books = all_books.Where(b => b.Shelves.Contains(application_model.CurrentShelf)).ToReactiveList();
            SelectedBook = Books.FirstOrDefault();

            Shelf = new ShelfViewModel(application_model.CurrentShelf);
        }
    }
}
