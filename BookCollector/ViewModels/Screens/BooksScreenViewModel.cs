using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
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

        private ICollectionView _Books;
        public ICollectionView Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        public BooksScreenViewModel(IApplicationModel application_model, IEventAggregator event_aggregator, ISnackbarMessageQueue message_queue)
        {
            this.application_model = application_model;
            this.event_aggregator = event_aggregator;
            this.message_queue = message_queue;

            DisplayName = Constants.BooksScreenDisplayName;

            this.WhenAnyValue(x => x.SelectedShelf)
                .Subscribe(_ => 
                {
                    Books?.Refresh();
                    Books?.MoveCurrentToFirst();
                });
        }

        public override void Activate()
        {
            base.Activate();

            var vms = application_model.CurrentCollection.Books.Select(b => new BookViewModel(b));
            Books = CollectionViewSource.GetDefaultView(vms);
            Books.MoveCurrentToFirst();

            Books.Filter = (o) =>
            {
                var book = o as BookViewModel;
                if (book == null || SelectedShelf == null)
                    return false;

                return book.Shelves.Any(s => s.Name == SelectedShelf.Name);
            };

            Books.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

            Shelves = application_model.CurrentCollection.Shelves.Select(s => new ShelfViewModel(s)).ToReactiveList();
            SelectedShelf = Shelves.FirstOrDefault();

            if (!application_model.CurrentCollection.Books.Any())
                message_queue.Enqueue("No Books?", "Import Here", () => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.ImportScreenDisplayName)));
        }
    }
}
