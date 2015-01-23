using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Apis;
using BookCollector.Model;
using BookCollector.Shell;
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

        private readonly IEventAggregator event_aggregator;
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
        public ImportViewModel(IEventAggregator event_aggregator, ProfileController profile_controller, BookRepository book_repository)
        {
            this.event_aggregator = event_aggregator;
            this.profile_controller = profile_controller;
            this.book_repository = book_repository;

            ResultsPart = new ImportResultsViewModel(this);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Messages.Clear();
            event_aggregator.PublishOnUIThread(ShellMessage.Text("Please select the source to import from"));
            SelectedIndex = ProgressPartIndex;

            var selection = IoC.Get<ImportSelectionViewModel>();
            var result = await DialogController.ShowViewModel(selection);

            if (result == MessageDialogResult.Negative)
                Cancel();
        }

        public void Cancel()
        {
            event_aggregator.PublishOnUIThread(ShellMessage.Back());
        }

        public void SelectController(IImportController import_controller)
        {
            event_aggregator.PublishOnUIThread(ShellMessage.Text("Importing from " + import_controller.ApiName));

            event_aggregator.PublishOnUIThread(ShellMessage.Busy(true));
            import_controller.Start(profile_controller.CurrentProfile);
        }

        public void UpdateProgress(string message)
        {
            Messages.Add(message);
        }

        public async void ShowResults(List<ImportedBook> books)
        {
            await Task.Delay(2000);
            event_aggregator.PublishOnUIThread(ShellMessage.Busy(false));
            SelectedIndex = ResultsPartIndex;

            var view_models = books.Select(b =>
            {
                var duplicate = book_repository.GetDuplicate(b.Book);
                return new ImportedBookViewModel(b)
                {
                    IsDuplicate = duplicate != null,
                    ImportSource = (duplicate == null ? "" : duplicate.ImportSource)
                };
            }).ToList();
            ResultsPart.Update(view_models);
        }

        public void ImportBooks(List<ImportedBook> books)
        {
            profile_controller.Import(books);
            Cancel();
        }
    }
}
