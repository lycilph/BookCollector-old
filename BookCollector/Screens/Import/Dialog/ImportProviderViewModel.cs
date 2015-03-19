using BookCollector.Api.ImportProvider;
using Panda.ApplicationCore.Utilities;

namespace BookCollector.Screens.Import.Dialog
{
    public class ImportProviderViewModel : ItemViewModelBase<IImportProvider>
    {
        public string DisplayName { get { return AssociatedObject.Name; } }

        public ImportProviderViewModel(IImportProvider obj) : base(obj) { }
    }
}
