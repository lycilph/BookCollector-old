using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using BookCollector.Controllers;
using GongSolutions.Wpf.DragDrop;
using Panda.ApplicationCore.Extensions;
using ReactiveUI;
using BookCollector.Data;
using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Screens.Main
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : ReactiveScreen, IDropTarget
    {
        private readonly INavigationController navigation_controller;
        private readonly IDataController data_controller;

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

        [ImportingConstructor]
        public MainViewModel(INavigationController navigation_controller, IDataController data_controller)
        {
            this.navigation_controller = navigation_controller;
            this.data_controller = data_controller;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Shelfs = data_controller.Collection.Shelfs.Select(s => new ShelfViewModel(s)).ToReactiveList();
            if (Shelfs != null && Shelfs.Any())
            {
                SelectedShelf = Shelfs.First();
                if (SelectedShelf.Books != null && SelectedShelf.Books.Any())
                    SelectedShelf.SelectedBook = SelectedShelf.Books.First();
            }
        }

        public void AddShelf()
        {
            Shelfs.Add(new ShelfViewModel(new Shelf {Name = "[Name]"}));
        }

        public void Search()
        {
            navigation_controller.NavigateToSearch();
        }

        public void DragOver(IDropInfo drop_info)
        {
            var source_item = drop_info.Data as BookViewModel;
            var target_item = drop_info.TargetItem as ShelfViewModel;

            if (source_item != null && target_item != null && target_item != SelectedShelf)
            {
                drop_info.DropTargetAdorner = DropTargetAdorners.Highlight;
                drop_info.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo drop_info)
        {
            var source_item = drop_info.Data as BookViewModel;
            var target_item = drop_info.TargetItem as ShelfViewModel;

            if (source_item != null && target_item != null)
            {
                SelectedShelf.Remove(source_item);
                target_item.Add(source_item);
            }
        }
    }
}
