using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using BookCollector.Controllers;
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
        private readonly ApplicationController application_controller;
        private readonly WindowCommand window_command;

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
        public ShellViewModel(IEventAggregator event_aggregator, ApplicationController application_controller)
        {
            this.application_controller = application_controller;

            window_command = new WindowCommand(string.Empty, application_controller.NavigateToProfiles);
            RightShellCommands.Add(window_command);

            this.WhenAnyValue(x => x.ActiveItem)
                .Where(x => x != null)
                .Subscribe(x => window_command.IsEnabled = x.IsCommandsEnabled);

            event_aggregator.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            logger.Trace("Shell initializing");
            DisplayName = "Book Collector";

            application_controller.Initialize();
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
                case ShellMessage.MessageKind.StatusText:
                    Text = message.Text;
                    break;
                case ShellMessage.MessageKind.CommandText:
                    window_command.DisplayName = message.Text;
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