using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Services.Import;
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

        private readonly ImportSelectionViewModel selection;
        private readonly ImportInformationViewModel information;
        private readonly ImportResultsViewModel results;
   
        [ImportingConstructor]
        public ImportViewModel(IEventAggregator event_aggregator, ImportSelectionViewModel selection, ImportInformationViewModel information, ImportResultsViewModel results)
        {
            this.selection = selection;
            this.information = information;
            this.results = results;

            Items.AddRange(new List<IScreen> {selection, information, results});

            event_aggregator.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(selection);
        }

        private void Select(IImportController controller)
        {
            logger.Trace("Selected the {0} import controller", controller.Name);
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
