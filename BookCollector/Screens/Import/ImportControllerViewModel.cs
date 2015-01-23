using BookCollector.Apis;
using Framework.Core.MVVM;

namespace BookCollector.Screens.Import
{
    public class ImportControllerViewModel : ItemViewModelBase<IImportController>
    {
        private readonly ImportSelectionViewModel selection_view_model;

        public string DisplayName { get { return AssociatedObject.ApiName; } }

        public ImportControllerViewModel(ImportSelectionViewModel selection_view_model, IImportController obj) : base(obj)
        {
            this.selection_view_model = selection_view_model;
        }

        public void Select()
        {
            selection_view_model.Select(AssociatedObject);
        }
    }
}
