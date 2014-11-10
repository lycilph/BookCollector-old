using System;
using System.Linq;
using BookCollector.Services;
using BookCollector.Services.Import;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace BookCollector.Import
{
    public sealed class ImportResultsStepViewModel : ReactiveScreen
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ImportViewModel parent;
        private readonly IProgress<ImportProgressStatus> progress;
        private IImporter importer;

        private bool _CanContinue;
        public bool CanContinue
        {
            get { return _CanContinue; }
            set { this.RaiseAndSetIfChanged(ref _CanContinue, value); }
        }

        private ReactiveList<ImportedBookViewModel> _Books = new ReactiveList<ImportedBookViewModel>();
        public ReactiveList<ImportedBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        public ImportResultsStepViewModel(ImportViewModel parent)
        {
            this.parent = parent;
            event_aggregator = IoC.Get<IEventAggregator>();
            DisplayName = "Results";
            progress = new Progress<ImportProgressStatus>(Update);
        }

        protected override async void OnViewReady(object view)
        {
            base.OnViewReady(view);

            if (importer == null)
                throw new Exception();

            CanContinue = false;
            await importer.GetBooksAsync();
            CanContinue = true;
        }

        public void Setup(IApi api)
        {
            importer = ImporterFactory.Create(api, progress);
            Books.Clear();
        }

        public void Continue()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.TextMessage(string.Empty, string.Empty));
            parent.Done();
        }

        private void Update(ImportProgressStatus status)
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.TextMessage("Importing", status.Message));

            if (status.Books == null || !status.Books.Any()) 
                return;

            using (Books.SuppressChangeNotifications())
            {
                Books.AddRange(status.Books.Select(b => new ImportedBookViewModel(b)));
            }
        }
    }
}
