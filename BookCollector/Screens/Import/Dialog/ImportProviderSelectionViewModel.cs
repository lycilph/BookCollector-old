using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Api.ImportProvider;
using Caliburn.Micro.ReactiveUI;
using MahApps.Metro.Controls.Dialogs;
using Panda.ApplicationCore.Dialogs;
using Panda.ApplicationCore.Utilities;
using ReactiveUI;

namespace BookCollector.Screens.Import.Dialog
{
    [Export(typeof(ImportProviderSelectionViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImportProviderSelectionViewModel : ReactiveScreen, IHaveDoneTask
    {
        private readonly TaskCompletionSource<MessageDialogResult> tcs = new TaskCompletionSource<MessageDialogResult>();
        public Task<MessageDialogResult> Done { get { return tcs.Task; } }

        private IImportProvider _SelectedImportProvider;
        public IImportProvider SelectedImportProvider
        {
            get { return _SelectedImportProvider; }
            set { this.RaiseAndSetIfChanged(ref _SelectedImportProvider, value); }
        }

        private ReactiveList<ImportProviderViewModel> _ImportProviders;
        public ReactiveList<ImportProviderViewModel> ImportProviders
        {
            get { return _ImportProviders; }
            set { this.RaiseAndSetIfChanged(ref _ImportProviders, value); }
        }

        [ImportingConstructor]
        public ImportProviderSelectionViewModel([ImportMany] IEnumerable<IImportProvider> providers)
        {
            ImportProviders = providers.Select(p => new ImportProviderViewModel(p)).ToReactiveList();
        }

        public void Cancel()
        {
            tcs.SetResult(MessageDialogResult.Negative);
        }

        public void Select(ImportProviderViewModel import_provider_view_model)
        {
            SelectedImportProvider = import_provider_view_model.AssociatedObject;
            tcs.SetResult(MessageDialogResult.Affirmative);
        }
    }
}
