using Framework.Core.MVVM;

namespace BookCollector.Import
{
    public class ImportControllerViewModel : ItemViewModelBase<IImportController>
    {
        public string DisplayName { get { return AssociatedObject.Name; } }

        public ImportControllerViewModel(IImportController obj) : base(obj)
        {
        }
    }
}
