using System;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Services.Books;
using BookCollector.Shell;
using BookCollector.Utilities;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace BookCollector.Screens.Import
{
    [Export(typeof(ImportResultsViewModel))]
    public class ImportResultsViewModel : ReactiveScreen, IHandle<ImportMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly BookRepository book_repository;

        private ReactiveList<ImportedBookViewModel> _Books = new ReactiveList<ImportedBookViewModel>();
        public ReactiveList<ImportedBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private bool _IsAllSelected;
        public bool IsAllSelected
        {
            get { return _IsAllSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsAllSelected, value); }
        }

        [ImportingConstructor]
        public ImportResultsViewModel(IEventAggregator event_aggregator, BookRepository book_repository)
        {
            this.event_aggregator = event_aggregator;
            this.book_repository = book_repository;

            event_aggregator.Subscribe(this);

            this.WhenAnyValue(x => x.IsAllSelected)
                .Subscribe(selected => Books.Apply(b => b.IsSelected = selected));
        }

        public void Ok()
        {
            var selected_books = Books.Where(b => b.IsSelected)
                                      .Select(b => b.AssociatedObject)
                                      .ToList();
            book_repository.Import(selected_books);

            Cancel();
        }

        public void Cancel()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.Back());
        }

        public void Handle(ImportMessage message)
        {
            if (message.Kind != ImportMessage.MessageKind.Results)
                return;

            // Reset selections
            IsAllSelected = false;

            logger.Trace("Got {0} books", message.ImportedBooks.Count);
            var duplicates = message.ImportedBooks.Select(b => book_repository.GetDuplicate(b.Book));
            Books = message.ImportedBooks.Zip(duplicates, (book, duplicate) => new ImportedBookViewModel(book, duplicate)).ToReactiveList();

            Books.Apply(b => b.IsSelected = !b.IsDuplicate);
            if (Books.All(b => b.IsSelected))
                IsAllSelected = true;
        }
    }
}