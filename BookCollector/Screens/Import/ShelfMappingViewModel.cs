using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Data;
using BookCollector.Data;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ShelfMappingViewModel : ReactiveObject
    {
        public string ImportedShelf { get; set; }
        public ICollectionView ExistingShelves { get; set; }
        public Shelf SelectedShelf { get { return (Shelf)ExistingShelves.CurrentItem; } }

        private ReactiveCommand _EditCommand;
        public ReactiveCommand EditCommand
        {
            get { return _EditCommand; }
            set { this.RaiseAndSetIfChanged(ref _EditCommand, value); }
        }

        private bool _IsEditing;
        public bool IsEditing
        {
            get { return _IsEditing; }
            set { this.RaiseAndSetIfChanged(ref _IsEditing, value); }
        }

        public string ShelfName
        {
            get { return SelectedShelf.Name; }
            set
            {
                this.RaisePropertyChanging();
                SelectedShelf.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public ShelfMappingViewModel(string imported_shelf, ReactiveList<Shelf> existing_shelves)
        {
            ImportedShelf = imported_shelf;
            ExistingShelves = new CollectionViewSource { Source = existing_shelves }.View;
            ExistingShelves.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ExistingShelves.MoveCurrentToFirst();

            var current_changed = Observable.FromEventPattern(x => ExistingShelves.CurrentChanged += x,
                                                              x => ExistingShelves.CurrentChanged -= x);

            var can_edit = current_changed.Select(_ => !SelectedShelf.IsDefault);
            EditCommand = ReactiveCommand.Create(() => IsEditing = !IsEditing, can_edit);

            // This is needed to fire the Changed event for the ShelfName
            current_changed.Subscribe(_ => ShelfName = SelectedShelf.Name);
        }

        public void SetCurrent(Shelf shelf)
        {
            ExistingShelves.MoveCurrentTo(shelf);
        }
    }
}
