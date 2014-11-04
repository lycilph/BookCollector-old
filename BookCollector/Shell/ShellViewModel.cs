using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Services;
using Caliburn.Micro;
using Framework.Core.Shell;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Shell
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ConductorShell<IScreen>, IHandle<ShellMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Stack<IScreen> items = new Stack<IScreen>();
        private readonly ApplicationSettings settings;
        private readonly BookRepository book_repository;
        private readonly IScreen main_view_model;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator event_aggregator, ApplicationSettings settings, BookRepository book_repository, [Import("Main")] IScreen main_view_model)
        {
            this.settings = settings;
            this.book_repository = book_repository;
            this.main_view_model = main_view_model;

            event_aggregator.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            logger.Trace("Shell initializing");

            base.OnInitialize();
            DisplayName = "Book Collector";

            settings.Load();
            book_repository.Load();
            Show(main_view_model);
        }

        protected override void OnDeactivate(bool close)
        {
            logger.Trace(string.Format("Shell deactivating ({0})", close));

            base.OnDeactivate(close);

            if (close)
            {
                settings.Save();
                book_repository.Save();
            }
        }

        protected void Back()
        {
            items.Pop();
            ActivateItem(items.Peek());
        }

        protected void Show(IScreen view_model)
        {
            items.Push(view_model);
            ActivateItem(view_model);
        }

        public void Handle(ShellMessage message)
        {
            switch (message.Kind)
            {
                case ShellMessage.MessageKind.Back:
                    Back();
                    break;
                case ShellMessage.MessageKind.Show:
                    Show(message.ViewModel);
                    break;
                case ShellMessage.MessageKind.Exit:
                    TryClose();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}