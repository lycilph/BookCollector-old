using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Apis;
using BookCollector.Utilities;
using Caliburn.Micro.ReactiveUI;
using Framework.Core.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    [Export(typeof(ImportSelectionViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImportSelectionViewModel : ReactiveScreen, IHaveDoneTask
    {
        private readonly IImportProcessController import_process_controller;

        private readonly TaskCompletionSource<MessageDialogResult> tcs = new TaskCompletionSource<MessageDialogResult>();
        public Task<MessageDialogResult> Done { get { return tcs.Task; } }

        private ReactiveList<ImportControllerViewModel> _ImportControllers;
        public ReactiveList<ImportControllerViewModel> ImportControllers
        {
            get { return _ImportControllers; }
            set { this.RaiseAndSetIfChanged(ref _ImportControllers, value); }
        }

        [ImportingConstructor]
        public ImportSelectionViewModel(ApiController api_controller, IImportProcessController import_process_controller)
        {
            this.import_process_controller = import_process_controller;
            ImportControllers = api_controller.GetImportControllers()
                                              .Select(i => new ImportControllerViewModel(this, i))
                                              .ToReactiveList();
        }

        public void Cancel()
        {
            tcs.SetResult(MessageDialogResult.Negative);
        }

        public void Select(IImportController import_controller)
        {
            import_process_controller.SelectController(import_controller);
            tcs.SetResult(MessageDialogResult.Affirmative);
        }
    }
}
