using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Data;
using BookCollector.Goodreads;
using Caliburn.Micro;
using Framework.Docking;
using ReactiveUI;

namespace BookCollector.Main
{
    [Export(typeof(IMain))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : ContentScreen, IMain
    {
        private readonly GoodreadsApi api;

        private BookCollection collection;
        private ConcurrentQueue<Book> queue;

        public Info Info { get; set; }

        private IReactiveDerivedList<BookViewModel> _Books;

        public IReactiveDerivedList<BookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        [ImportingConstructor]
        public MainViewModel(GoodreadsApi api)
        {
            this.api = api;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = Info.DisplayName;

            collection = Info.Load();
            Books = collection.Books.CreateDerivedCollection(b => new BookViewModel(b));

            Task.Factory
                .StartNew(() => api.RequestUpdateQueue())
                .ContinueWith(parent =>
                {
                    queue = parent.Result;

                    var books_to_update = collection.Books.Where(b => b.Status == BookStatus.Ready).ToList();
                    if (books_to_update.Any())
                        books_to_update.Apply(b => queue.Enqueue(b));

                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            Info.Text = string.Format("Books {0}", collection.Books.Count);

            if (close)
            {
                Info.Save(collection);
                Task.Factory.StartNew(() => api.ShutdownUpdateQueue(queue));
            }
        }

        public void Import()
        {
            var books = GoodreadsImporter.Import();
            collection.Books.AddRange(books);

            var books_to_update = collection.Books.Where(b => b.Status == BookStatus.Ready).ToList();
            if (books_to_update.Any())
                books_to_update.Apply(b => queue.Enqueue(b));
        }
    }
}
