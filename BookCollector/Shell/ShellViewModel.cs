using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Model;
using Caliburn.Micro;
using Framework.Core.Shell;
using ReactiveUI;
using NLog;
using LogManager = NLog.LogManager;
using IScreen = Caliburn.Micro.IScreen;

namespace BookCollector.Shell
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ConductorShell<IScreen>, IHandle<ShellMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Stack<IScreen> items = new Stack<IScreen>();
        private readonly IEventAggregator event_aggregator;
        private readonly IWindowCommand collections_window_command;

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator event_aggregator, ProfileController profile_controller)
        {
            this.event_aggregator = event_aggregator;

            collections_window_command = new WindowCommand("Click here", () => logger.Trace("Command clicked"));
            RightShellCommands.Add(collections_window_command);

            //profile_controller.WhenAnyValue(x => x.)

            event_aggregator.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            logger.Trace("Shell initializing");
            DisplayName = "Book Collector";

            event_aggregator.PublishOnUIThread(ApplicationMessage.ShellInitialized());
        }

        protected override void OnActivate()
        {                   
            base.OnActivate();

            logger.Trace("Shell activating");
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            logger.Trace(string.Format("Shell deactivating ({0})", close));
        }

        protected override void OnActivationProcessed(IScreen item, bool success)
        {
            base.OnActivationProcessed(item, success);

            logger.Trace("Activation of {0} processed", item.DisplayName);
        }

        private void Back()
        {
            items.Pop();
            ActivateItem(items.Peek());
        }

        private void Show(IScreen view_model)
        {
            items.Push(view_model);
            ActivateItem(view_model);
        }

        public void Handle(ShellMessage message)
        {
            logger.Trace("Shellmessage (Kind = {0})", Enum.GetName(typeof(ShellMessage.MessageKind), message.Kind));

            switch (message.Kind)
            {
                case ShellMessage.MessageKind.Back:
                    Back();
                    break;
                case ShellMessage.MessageKind.Show:
                    Show(message.ViewModel);
                    break;
                case ShellMessage.MessageKind.Text:
                    Text = message.Message;
                    break;
                case ShellMessage.MessageKind.Busy:
                    IsBusy = message.State;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}