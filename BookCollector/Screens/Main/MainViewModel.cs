using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using BookCollector.Controllers;
using GongSolutions.Wpf.DragDrop;
using ReactiveUI;
using BookCollector.Data;

namespace BookCollector.Screens.Main
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : BookCollectorScreenBase, IDropTarget
    {
        private readonly INavigationController navigation_controller;
        private readonly IDataController data_controller;

        private ShelfViewModel _SelectedShelf;
        public ShelfViewModel SelectedShelf
        {
            get { return _SelectedShelf; }
            set { this.RaiseAndSetIfChanged(ref _SelectedShelf, value); }
        }

        private IReactiveDerivedList<ShelfViewModel> _Shelves;
        public IReactiveDerivedList<ShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
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

            Shelves = data_controller.Collection.Shelves.CreateDerivedCollection(s => new ShelfViewModel(s));
            if (Shelves != null && Shelves.Any())
            {
                SelectedShelf = Shelves.First();
                if (SelectedShelf.Books != null && SelectedShelf.Books.Any())
                    SelectedShelf.SelectedBook = SelectedShelf.Books.First();
            }
        }

        public void AddShelf()
        {
            data_controller.Collection.Add(new Shelf {Name = "[Name]"});
        }

        public void Search()
        {
            navigation_controller.NavigateToSearch();
        }

        public void Import()
        {
            navigation_controller.NavigateToImport();
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
