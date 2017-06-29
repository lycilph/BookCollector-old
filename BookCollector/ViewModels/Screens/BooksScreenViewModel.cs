using BookCollector.Domain;
using BookCollector.Framework.MVVM;

namespace BookCollector.ViewModels.Screens
{
    public class BooksScreenViewModel : ScreenBase
    {
        public BooksScreenViewModel()
        {
            DisplayName = Constants.BooksScreenDisplayName;
        }

        //private IApplicationModel application_model;

        //private IReactiveDerivedList<ShelfSelectionViewModel> _BookAndShelves;
        //public IReactiveDerivedList<ShelfSelectionViewModel> BookAndShelves
        //{
        //    get { return _BookAndShelves; }
        //    set { this.RaiseAndSetIfChanged(ref _BookAndShelves, value); }
        //}

        //public BooksScreenViewModel(IApplicationModel application_model)
        //{
        //    this.application_model = application_model;

        //    BookAndShelves = application_model.CurrentCollection.Shelves.CreateDerivedCollection(s => new ShelfSelectionViewModel(null, s));
        //}

        //public override void Activate()
        //{
        //    base.Activate();
        //}
    }
}
