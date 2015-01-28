using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
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
        private readonly RepositorySearchProvider search_provider;
        private ICollectionView collection_view;
        private List<string> filter_list = new List<string>();

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

        private string _Query;
        public string Query
        {
            get { return _Query; }
            set { this.RaiseAndSetIfChanged(ref _Query, value); }
        }
        
        [ImportingConstructor]
        public MainViewModel(ApplicationController application_controller)
        {
            this.application_controller = application_controller;

            this.WhenAnyValue(x => x.Query)
                .Subscribe(Search);

            book_repository = application_controller.BookRepository;
            search_provider = application_controller.RepositorySearchProvider;
        }

        private async void Search(string query)
        {
            if (collection_view == null)
                return;

            if (string.IsNullOrWhiteSpace(query))
            {
                filter_list.Clear();
                collection_view.Filter = null;
                UpdateStatusText();
            }
            else
            {
                filter_list = await Task.Factory.StartNew(() => search_provider.Search(query));
                collection_view.Filter = Filter;
                application_controller.SetStatusText(string.Format("Found {0} books", filter_list.Count));
            }

            collection_view.Refresh();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            logger.Trace("Activating");

            Books = book_repository.Books.OrderBy(b => b.Title).Select(b => new MainBookViewModel(b)).ToList();
            collection_view = CollectionViewSource.GetDefaultView(Books);

            UpdateStatusText();
            SelectedBook = Books.FirstOrDefault();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            logger.Trace("Deactivating");

            Query = string.Empty;
        }

        private void UpdateStatusText()
        {
            var text = (Books.Any() ? "Books: " + Books.Count : "No books");
            application_controller.SetStatusText(text);
        }

        private bool Filter(object o)
        {
            if (!filter_list.Any())
                return true;

            var book = o as MainBookViewModel;
            return book != null && filter_list.Contains(book.AssociatedObject.Id);
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
