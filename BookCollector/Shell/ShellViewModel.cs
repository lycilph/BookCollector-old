using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using BookCollector.Model;
using BookCollector.Screens;
using Caliburn.Micro;
using Framework.Core.Shell;
using ReactiveUI;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Shell
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ConductorShell<IShellScreen>, IHandle<ShellMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Stack<IShellScreen> items = new Stack<IShellScreen>();
        private readonly IEventAggregator event_aggregator;

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

            var cmd = new WindowCommand(string.Empty, () => event_aggregator.PublishOnUIThread(ApplicationMessage.ShowProfiles()));
            RightShellCommands.Add(cmd);

            profile_controller.WhenAnyValue(x => x.CurrentProfile.DisplayName, x => x.CurrentCollection.DisplayName)
                              .Where(pair => pair.Item1 != null && pair.Item2 != null)
                              .Subscribe(x =>
                              {
                                  var name = string.Format("{0} [{1}]", profile_controller.CurrentProfile.DisplayName, profile_controller.CurrentCollection.DisplayName);
                                  cmd.DisplayName = name;
                              });

            this.WhenAnyValue(x => x.ActiveItem)
                .Where(x => x != null)
                .Subscribe(x => cmd.IsEnabled = x.IsCommandsEnabled);

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

        protected override void OnActivationProcessed(IShellScreen item, bool success)
        {
            base.OnActivationProcessed(item, success);

            logger.Trace("Activation of {0} processed", item.DisplayName);
        }

        private void Back()
        {
            items.Pop();
            ActivateItem(items.Peek());
        }

        private void Show(IShellScreen view_model)
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