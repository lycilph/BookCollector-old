using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Utilities;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace BookCollector.Import
{
    [Export(typeof(ImportSelectionViewModel))]
    public class ImportSelectionViewModel : ReactiveScreen
    {
        private ReactiveList<ImportControllerViewModel> _ImportControllers;
        public ReactiveList<ImportControllerViewModel> ImportControllers
        {
            get { return _ImportControllers; }
            set { this.RaiseAndSetIfChanged(ref _ImportControllers, value); }
        }

        [ImportingConstructor]
        public ImportSelectionViewModel([ImportMany] IEnumerable<IImportController> import_controllers)
        {
            ImportControllers = import_controllers.Select(i => new ImportControllerViewModel(i)).ToReactiveList();
        }
    }
}
