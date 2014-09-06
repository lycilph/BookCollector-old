using System.ComponentModel.Composition;
using BookCollector.Data;
using BookCollector.Goodreads;
using Framework.Docking;
using ReactiveUI;

namespace BookCollector.Main
{
    [Export(typeof(IMain))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : ContentScreen, IMain
    {
        private BookCollection collection;

        public Info Info { get; set; }

        private IReactiveDerivedList<BookViewModel> _Books;
        public IReactiveDerivedList<BookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = Info.DisplayName;

            collection = Info.Load();
            Books = collection.Books.CreateDerivedCollection(b => new BookViewModel(b));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            Info.Text = string.Format("Books {0}", collection.Books.Count);

            if (close)
                Info.Save(collection);
        }

        public void Import()
        {
            var books = GoodreadsImporter.Import();
            collection.Books.AddRange(books);                
        }
    }
}
