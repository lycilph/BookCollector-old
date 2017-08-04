using System.ComponentModel;
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

        public ShelfMappingViewModel(string imported_shelf, ReactiveList<Shelf> existing_shelves)
        {
            ImportedShelf = imported_shelf;
            ExistingShelves = new CollectionViewSource { Source = existing_shelves }.View;
            ExistingShelves.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ExistingShelves.MoveCurrentToFirst();
        }

        public void SetCurrent(Shelf shelf)
        {
            ExistingShelves.MoveCurrentTo(shelf);
        }
    }
}
