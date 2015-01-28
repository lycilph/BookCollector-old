using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Controllers;
using NLog;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace BookCollector.Screens.Main
{
    [Export("Main", typeof(IShellScreen))]
    public class MainViewModel : ShellScreenBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ApplicationController application_controller;
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
        public MainViewModel(ApplicationController application_controller)
        {
            this.application_controller = application_controller;

            book_repository = application_controller.BookRepository;
        }

        protected override void OnActivate()
        {
            logger.Trace("Activating");

            Books = book_repository.Books.OrderBy(b => b.Title).Select(b => new MainBookViewModel(b)).ToList();
            if (Books.Any())
            {
                SelectedBook = Books.First();
                application_controller.SetStatusText("Books: " + Books.Count);
            }
            else
            {
                SelectedBook = null;
                application_controller.SetStatusText("No books");
            }
        }

        public void Import()
        {
            application_controller.NavigateToImport();
        }

        public void Settings()
        {
            application_controller.NavigateToSettings();
        }
    }
}
