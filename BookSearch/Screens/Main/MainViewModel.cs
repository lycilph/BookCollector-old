using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BookSearch.Api.SearchProvider;
using BookSearch.Data;
using BookSearch.Services.Scoring;
using BookSearch.Shell;
using Caliburn.Micro.ReactiveUI;
using Microsoft;
using NLog;
using Panda.ApplicationCore.Extensions;
using Panda.Utilities.Extensions;
using ReactiveUI;

namespace BookSearch.Screens.Main
{
    public class MainViewModel : ReactiveScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ShellViewModel shell;
        private readonly List<Book> books;
        private Similarity similarity;

        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set { this.RaiseAndSetIfChanged(ref _SearchText, value); }
        }

        private DocumentViewModel _CurrentDocument;
        public DocumentViewModel CurrentDocument
        {
            get { return _CurrentDocument; }
            set { this.RaiseAndSetIfChanged(ref _CurrentDocument, value); }
        }

        private ReactiveList<DocumentViewModel> _Documents;
        public ReactiveList<DocumentViewModel> Documents
        {
            get { return _Documents; }
            set { this.RaiseAndSetIfChanged(ref _Documents, value); }
        }

        private ReactiveList<SearchProviderViewModel> _SearchProviders;
        public ReactiveList<SearchProviderViewModel> SearchProviders
        {
            get { return _SearchProviders; }
            set { this.RaiseAndSetIfChanged(ref _SearchProviders, value); }
        }

        public MainViewModel(ShellViewModel shell)
        {
            this.shell = shell;

            var results = new Progress<List<Book>>(Update);
            SearchProviders = new ReactiveList<SearchProviderViewModel>
            {
                new SearchProviderViewModel(new AmazonSearchProvider(results), "Amazon-icon.png"),
                new SearchProviderViewModel(new GoodreadsSearchProvider(results), "Goodreads-icon.png"),
                new SearchProviderViewModel(new GoogleBooksSearchProvider(results), "Google-Play-Books-icon.png")
            };
            books = new List<Book>();
        }

        public async void Search(Key key)
        {
            if (key != Key.Enter) 
                return;

            shell.MainStatusText = SearchText;
            shell.IsBusy = true;

            books.Clear();
            similarity = new Similarity(SearchText);

            var sw = Stopwatch.StartNew();
            var tasks = SearchProviders.Select(s => s.Search(SearchText)).ToList();
            await Task.WhenAll(tasks);
            var elapsed = sw.StopAndGetElapsedMilliseconds();

            shell.IsBusy = false;
            shell.MainStatusText = string.Format("Found {0} books, search took {1} ms", books.Count, elapsed);
        }

        private void Update(IEnumerable<Book> search_results)
        {
            var sw = Stopwatch.StartNew();
            
            books.AddRange(search_results);
            var documents = similarity.Score(books);
            Documents = documents.OrderByDescending(d => d.Score)
                                 .Select(d => new DocumentViewModel(d))
                                 .ToReactiveList();

            var elapsed = sw.StopAndGetElapsedMilliseconds();
            shell.AuxiliaryStatusText = string.Format("Scored in {0} ms", elapsed);
        }
    }
}
