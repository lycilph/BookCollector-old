using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Apis;
using BookCollector.Model;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Screens.Import
{
    [Export("Import", typeof(IShellScreen))]
    public class ImportViewModel : ReactiveConductor<IScreen>.Collection.OneActive, IShellScreen, IHandle<ImportMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly ProfileController profile_controller;
        private readonly ImportSelectionViewModel selection;
        private readonly ImportInformationViewModel information;
        private readonly ImportResultsViewModel results;

        public bool IsCommandsEnabled { get { return true; } }
   
        [ImportingConstructor]
        public ImportViewModel(ImportSelectionViewModel selection, ImportInformationViewModel information, ImportResultsViewModel results, IEventAggregator event_aggregator, ProfileController profile_controller)
        {
            this.selection = selection;
            this.information = information;
            this.results = results;
            this.event_aggregator = event_aggregator;
            this.profile_controller = profile_controller;

            Items.AddRange(new List<IScreen> {selection, information, results});

            event_aggregator.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(selection);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            event_aggregator.PublishOnUIThread(ShellMessage.Text(string.Empty));
        }

        private void Select(IImportController import_controller)
        {
            event_aggregator.PublishOnUIThread(ShellMessage.Text("Importing from " + import_controller.ApiName));

            ActivateItem(information);

            var profile = profile_controller.CurrentProfile;
            import_controller.Start(profile);
        }

        public void Back()
        {
            event_aggregator.PublishOnUIThread(ShellMessage.Back());            
        }

        public void Handle(ImportMessage message)
        {
            switch (message.Kind)
            {
                case ImportMessage.MessageKind.Select:
                    Select(message.ImportController);
                    break;
                case ImportMessage.MessageKind.Results:
                    ActivateItem(results);
                    break;
            }
        }
    }
}
