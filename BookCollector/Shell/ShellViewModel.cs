using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using BookCollector.Services.Collections;
using BookCollector.Services.Settings;
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

        private readonly ApplicationSettings settings;
        private readonly CollectionsController collections_controller;
        private readonly Stack<IScreen> items = new Stack<IScreen>();
        private readonly IScreen main_view_model;
        private readonly IScreen collections_view_model;
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
        public ShellViewModel(IEventAggregator event_aggregator,
                              ApplicationSettings settings,
                              CollectionsController collections_controller,
                              [Import("Main")] IScreen main_view_model,
                              [Import("Collections")] IScreen collections_view_model)
        {
            this.main_view_model = main_view_model;
            this.collections_view_model = collections_view_model;
            this.settings = settings;
            this.collections_controller = collections_controller;

            collections_window_command = new WindowCommand("", () => Show(collections_view_model));
            RightShellCommands.Add(collections_window_command);

            event_aggregator.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            logger.Trace("Shell initializing");
            DisplayName = "Book Collector";
            Show(main_view_model);

            if (!settings.RememberLastCollection || collections_controller.Current == null)
                Show(collections_view_model);
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

            var visible = (item != collections_view_model);
            var enabled = (item == main_view_model);
            RightShellCommands.Apply(cmd =>
            {
                cmd.IsVisible = visible;
                cmd.IsEnabled = enabled;
            });

            collections_window_command.DisplayName = (collections_controller.Current == null ? "" : collections_controller.Current.DisplayName);
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