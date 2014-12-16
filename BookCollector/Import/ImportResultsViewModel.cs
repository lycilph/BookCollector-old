using System;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Services;
using BookCollector.Shell;
using BookCollector.Utilities;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace BookCollector.Import
{
    [Export(typeof(ImportResultsViewModel))]
    public class ImportResultsViewModel : ReactiveScreen, IHandle<ImportMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly BookRepository book_repository;
        private readonly Downloader downloader;

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
        public ImportResultsViewModel(IEventAggregator event_aggregator, BookRepository book_repository, Downloader downloader)
        {
            this.event_aggregator = event_aggregator;
            this.book_repository = book_repository;
            this.downloader = downloader;

            event_aggregator.Subscribe(this);

            this.WhenAnyValue(x => x.IsAllSelected)
                .Subscribe(selected => Books.Apply(b => b.IsSelected = selected));
        }

        public void Ok()
        {
            var selected_books = Books.Where(b => b.IsSelected).ToList();
            book_repository.AddRange(selected_books.Select(b => b.AssociatedObject.Book));
            downloader.AddRange(selected_books.Select(b => b.AssociatedObject));

            Cancel();           
        }

        public void Cancel()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.BackMessage());
        }

        public void Handle(ImportMessage message)
        {
            if (message.Kind != ImportMessage.MessageKind.Results)
                return;

            // Reset selections
            IsAllSelected = false;

            logger.Trace("Got {0} books", message.ImportedBooks.Count);
            Books = message.ImportedBooks.Select(b => new ImportedBookViewModel(b) { IsDuplicate = book_repository.IsDuplicate(b.Book) }).ToReactiveList();

            Books.Apply(b => b.IsSelected = !b.IsDuplicate);
            if (Books.All(b => b.IsSelected))
                IsAllSelected = true;
        }
    }
}