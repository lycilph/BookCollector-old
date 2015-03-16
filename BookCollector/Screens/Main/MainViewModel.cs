using System.ComponentModel;
using System.ComponentModel.Composition;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using BookCollector.Controllers;
using GongSolutions.Wpf.DragDrop;
using NLog;
using Panda.ApplicationCore.Dialogs;
using ReactiveUI;
using BookCollector.Data;
using LogManager = NLog.LogManager;

namespace BookCollector.Screens.Main
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : BookCollectorScreenBase, IDropTarget
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly INavigationController navigation_controller;
        private readonly IDataController data_controller;
        private IDisposable view_source_subscription;

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

            var view_source = CollectionViewSource.GetDefaultView(Shelves);
            view_source.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));

            Shelves.ChangeTrackingEnabled = true;
            view_source_subscription = Shelves.ItemChanged.Where(x => x.PropertyName == "Name").Subscribe(x => view_source.Refresh());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            view_source_subscription.Dispose();
            view_source_subscription = null;
        }

        public void AddShelf()
        {
            data_controller.Collection.Add(new Shelf());
        }

        public void RemoveShelf()
        {
            data_controller.Collection.Remove(SelectedShelf.AssociatedObject);
        }

        public void EditShelf(ShelfViewModel shelf)
        {
            shelf.IsEditing = true;
        }

        public void Search()
        {
            navigation_controller.NavigateToSearch();
        }

        public void Import()
        {
            navigation_controller.NavigateToImport();
        }

        public async void Export()
        {
            await DialogController.ShowMessageAsync("Not implemented yet", "The export feature is not implemented yet");
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
