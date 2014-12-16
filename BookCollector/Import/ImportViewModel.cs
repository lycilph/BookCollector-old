using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Services.Import;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using IScreen = Caliburn.Micro.IScreen;
using LogManager = NLog.LogManager;

namespace BookCollector.Import
{
    [Export("Import", typeof(IScreen))]
    public class ImportViewModel : ReactiveConductor<IScreen>.Collection.OneActive, IHandle<ImportMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly ImportSelectionViewModel selection;
        private readonly ImportInformationViewModel information;
        private readonly ImportResultsViewModel results;
   
        [ImportingConstructor]
        public ImportViewModel(ImportSelectionViewModel selection, ImportInformationViewModel information, ImportResultsViewModel results, IEventAggregator event_aggregator)
        {
            this.selection = selection;
            this.information = information;
            this.results = results;
            this.event_aggregator = event_aggregator;

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
            event_aggregator.PublishOnUIThread(ShellMessage.TextMessage(string.Empty));
        }

        private void Select(IImportController controller)
        {
            event_aggregator.PublishOnUIThread(ShellMessage.TextMessage("Importing from " + controller.Name));

            ActivateItem(information);
            controller.Start();
        }

        public void Handle(ImportMessage message)
        {
            switch (message.Kind)
            {
                case ImportMessage.MessageKind.Selection:
                    Select(message.ImportController);
                    break;
                case ImportMessage.MessageKind.Results:
                    ActivateItem(results);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
