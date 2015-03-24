using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BookCollector.Api.SearchProvider;
using BookCollector.Controllers;
using BookCollector.Data;
using BookCollector.Services.DocumentScoring;
using Panda.ApplicationCore.Utilities;
using Panda.Utilities.Extensions;
using ReactiveUI;

namespace BookCollector.Screens.Search
{
    [Export(typeof(SearchViewModel))]
    public class SearchViewModel : BookCollectorScreenBase
    {
        private readonly IStatusController status_controller;
        private readonly INavigationController navigation_controller;
        private readonly IDataController data_controller;
        private readonly List<Book> books = new List<Book>();
        private Similarity similarity;

        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set { this.RaiseAndSetIfChanged(ref _SearchText, value); }
        }

        private DocumentViewModel _CurrentResultDocument;
        public DocumentViewModel CurrentResultDocument
        {
            get { return _CurrentResultDocument; }
            set { this.RaiseAndSetIfChanged(ref _CurrentResultDocument, value); }
        }

        private ReactiveList<DocumentViewModel> _ResultDocuments;
        public ReactiveList<DocumentViewModel> ResultDocuments
        {
            get { return _ResultDocuments; }
            set { this.RaiseAndSetIfChanged(ref _ResultDocuments, value); }
        }

        private ReactiveList<SearchProviderViewModel> _SearchProviders;
        public ReactiveList<SearchProviderViewModel> SearchProviders
        {
            get { return _SearchProviders; }
            set { this.RaiseAndSetIfChanged(ref _SearchProviders, value); }
        }

        [ImportingConstructor]
        public SearchViewModel(IStatusController status_controller, 
                               INavigationController navigation_controller, 
                               IDataController data_controller, 
                               [ImportMany] IEnumerable<ISearchProvider> search_providers)
        {
            this.status_controller = status_controller;
            this.navigation_controller = navigation_controller;
            this.data_controller = data_controller;

            SearchProviders = search_providers.Select(s => new SearchProviderViewModel(s)).ToReactiveList();
        }

        private void Update(IEnumerable<Book> search_results)
        {
            var sw = Stopwatch.StartNew();

            books.AddRange(search_results);
            var documents = similarity.Score(books);
            ResultDocuments = documents.OrderByDescending(d => d.Score)
                                       .Select(d => new DocumentViewModel(d) { Shelves = data_controller.Collection.Shelves })
                                       .ToReactiveList();

            if (CurrentResultDocument == null && ResultDocuments != null && ResultDocuments.Any())
                CurrentResultDocument = ResultDocuments.First();

            var elapsed = sw.StopAndGetElapsedMilliseconds();
            status_controller.AuxiliaryStatusText = string.Format("Scored in {0} ms", elapsed);
        }

        public async void Search(Key key)
        {
            if (key != Key.Enter)
                return;

            status_controller.MainStatusText = SearchText;
            status_controller.IsBusy = true;

            books.Clear();
            CurrentResultDocument = null;
            similarity = new Similarity(SearchText);

            var sw = Stopwatch.StartNew();
            var tasks = SearchProviders.Select(
                s => s.Search(SearchText)
                      .ContinueWith(parent => Update(parent.Result), TaskScheduler.FromCurrentSynchronizationContext()))
                .ToList();
            await Task.WhenAll(tasks);
            var elapsed = sw.StopAndGetElapsedMilliseconds();

            status_controller.IsBusy = false;
            status_controller.MainStatusText = string.Format("Found {0} books in {1} ms", books.Count, elapsed);
        }

        public void Back()
        {
            navigation_controller.Back();
        }
    }
}
