using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Services.Books;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;
using LogManager = NLog.LogManager;

namespace BookCollector.Screens.Main
{
    [Export("Main", typeof(IScreen))]
    public class MainViewModel : ReactiveScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly BookRepository book_repository;

        private List<MainBookViewModel> _Books;
        public List<MainBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private MainBookViewModel _SelectedBook;
        public MainBookViewModel SelectedBook
        {
            get { return _SelectedBook; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBook, value); }
        }
        
        [ImportingConstructor]
        public MainViewModel(IEventAggregator event_aggregator, BookRepository book_repository)
        {
            this.event_aggregator = event_aggregator;
            this.book_repository = book_repository;
        }

        protected override void OnActivate()
        {
            logger.Trace("Activating");

            Books = book_repository.Books.OrderBy(b => b.Title).Select(b => new MainBookViewModel(b)).ToList();

            if (Books.Any())
                SelectedBook = Books.First();

            if (Books.Count > 1)
                event_aggregator.PublishOnUIThread(ShellMessage.Text("Books: " + Books.Count));
        }

        public void Import()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.Show("Import"));
        }

        public void Settings()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.Show("Settings"));
        }
    }
}
