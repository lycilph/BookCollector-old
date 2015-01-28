using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Apis;
using BookCollector.Controllers;
using BookCollector.Model;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Framework.Core.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    [Export("Import", typeof(IShellScreen))]
    [Export(typeof(IImportProcessController))]
    public class ImportViewModel : ReactiveScreen, IShellScreen, IImportProcessController
    {
        private const int ProgressPartIndex = 0;
        private const int ResultsPartIndex = 1;

        private readonly ApplicationController application_controller;
        private readonly ProfileController profile_controller;
        private readonly BookRepository book_repository;

        public bool IsCommandsEnabled { get { return true; } }

        public ImportResultsViewModel ResultsPart { get; set; }

        private ReactiveList<string> _Messages = new ReactiveList<string>();
        public ReactiveList<string> Messages
        {
            get { return _Messages; }
            set { this.RaiseAndSetIfChanged(ref _Messages, value); }
        }

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { this.RaiseAndSetIfChanged(ref _SelectedIndex, value); }
        }

        [ImportingConstructor]
        public ImportViewModel(ApplicationController application_controller)
        {
            this.application_controller = application_controller;

            profile_controller = application_controller.ProfileController;
            book_repository = application_controller.BookRepository;

            ResultsPart = new ImportResultsViewModel(this);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Messages.Clear();
            application_controller.SetStatusText("Please select the source to import from");
            SelectedIndex = ProgressPartIndex;

            var selection = IoC.Get<ImportSelectionViewModel>();
            var result = await DialogController.ShowViewModel(selection);

            if (result == MessageDialogResult.Negative)
                Cancel();
        }

        public void Cancel()
        {
            application_controller.NavigateBack();
        }

        public void SelectController(IImportController import_controller)
        {
            application_controller.SetStatusText("Importing from " + import_controller.ApiName);
            application_controller.SetBusy(true);
            import_controller.Start(profile_controller.CurrentProfile);
        }

        public void UpdateProgress(string message)
        {
            Messages.Add(message);
        }

        public async void ShowResults(List<ImportedBook> books)
        {
            application_controller.SetBusy(false);
            var view_models = await Task.Factory.StartNew(() => books.Select(b =>
            {
                var duplicate = book_repository.GetDuplicate(b.Book);
                return new ImportedBookViewModel(b)
                {
                    IsDuplicate = duplicate != null,
                    ImportSource = (duplicate == null ? "" : duplicate.ImportSource)
                };
            }).ToList());
            ResultsPart.Update(view_models);

            await Task.Delay(2000);
            SelectedIndex = ResultsPartIndex;
        }

        public void ImportBooks(List<ImportedBook> books)
        {
            application_controller.Import(books);
            Cancel();
        }
    }
}
