using System.ComponentModel.Composition;
using System.Linq;
using ReactiveUI;
using BookCollector.Data;
using BookCollector.Services;
using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Screens.Main
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : ReactiveScreen
    {
        private ShelfViewModel _SelectedShelf;
        public ShelfViewModel SelectedShelf
        {
            get { return _SelectedShelf; }
            set { this.RaiseAndSetIfChanged(ref _SelectedShelf, value); }
        }

        private ReactiveList<ShelfViewModel> _Shelfs;
        public ReactiveList<ShelfViewModel> Shelfs
        {
            get { return _Shelfs; }
            set { this.RaiseAndSetIfChanged(ref _Shelfs, value); }
        }

        public MainViewModel()
        {
            const string csv_filename = @"C:\Private\Projects\BookCollector\Data\goodreads_export.csv";
            var shelf = new Shelf
            {
                Name = "All",
                Books = Importer.Read(csv_filename).ToList()
            };
            Shelfs = new ReactiveList<ShelfViewModel> { new ShelfViewModel(shelf) };
        }

        public void AddShelf()
        {
            Shelfs.Add(new ShelfViewModel(new Shelf {Name = "[Name]"}));
        }
    }
}
