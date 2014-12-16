using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Services;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace BookCollector.Main
{
    [Export("Main", typeof(IScreen))]
    public class MainViewModel : ReactiveScreen
    {
        private readonly IEventAggregator event_aggregator;
        private readonly BookRepository book_repository;
        private readonly IScreen import_view_model;
        private readonly IScreen settings_view_model;

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
        public MainViewModel([Import("Import")] IScreen import_view_model,
                             [Import("Settings")] IScreen settings_view_model,
                             IEventAggregator event_aggregator, 
                             BookRepository book_repository)
        {
            this.import_view_model = import_view_model;
            this.settings_view_model = settings_view_model;
            this.event_aggregator = event_aggregator;
            this.book_repository = book_repository;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Books = book_repository.Books.OrderBy(b => b.Title).Select(b => new MainBookViewModel(b)).ToList();

            if (Books.Any())
                SelectedBook = Books.First();

            if (Books.Count > 1)
                event_aggregator.PublishOnUIThread(ShellMessage.TextMessage("Books: " + Books.Count));
        }

        public void Import()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(import_view_model));
        }

        public void Settings()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(settings_view_model));
        }
    }
}
