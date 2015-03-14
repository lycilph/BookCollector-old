using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Windows;
using BookCollector.Screens;
using BookCollector.Shell;
using Caliburn.Micro;
using Panda.ApplicationCore.Shell;
using Panda.ApplicationCore.StatusBar.ViewModels;
using ReactiveUI;

namespace BookCollector.Controllers
{
    [Export(typeof(IStatusController))]
    public class StatusController : ReactiveObject, IStatusController, IHandle<ShellMessage>
    {
        private readonly IBookCollectorShell shell;
        private readonly StatusBarTextItemViewModel main_status_text = new StatusBarTextItemViewModel();
        private readonly StatusBarTextItemViewModel aux_status_text = new StatusBarTextItemViewModel(GridLength.Auto);
        private readonly StatusBarProgressItemViewModel progress_status = new StatusBarProgressItemViewModel(true, new GridLength(200));
        private readonly IWindowCommand selection_command;

        public string MainStatusText
        {
            set { main_status_text.Message = value; }
        }

        public string AuxiliaryStatusText
        {
            set { aux_status_text.Message = value; }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                this.RaiseAndSetIfChanged(ref _IsBusy, value);
                progress_status.IsActive = value;
            }
        }

        [ImportingConstructor]
        public StatusController(IEventAggregator event_aggregator, IBookCollectorShell shell, INavigationController navigation_controller, IDataController data_controller)
        {
            this.shell = shell;

            shell.StatusBar.Add(main_status_text, aux_status_text, progress_status);

            selection_command = new WindowCommand(string.Empty, navigation_controller.NavigateToSelection);
            shell.RightShellCommands.Add(selection_command);

            data_controller.WhenAnyValue(x => x.User.Name, x => x.Collection.Name)
                           .Select(x => string.Format("{0} - {1}", x.Item1, x.Item2))
                           .Subscribe(x => selection_command.DisplayName = x);

            event_aggregator.Subscribe(this);
        }

        public void ClearStatusText()
        {
            MainStatusText = string.Empty;
            AuxiliaryStatusText = string.Empty;
        }

        public void UpdateShowCurrentUser()
        {
            var screen = shell.ActiveItem as IBookCollectorScreen;
            if (screen == null)
                return;

            selection_command.IsVisible = screen.ShowCurrentUser;
        }

        public void Handle(ShellMessage message)
        {
            switch (message)
            {
                case ShellMessage.ActiveItemChanged:
                    ClearStatusText();
                    UpdateShowCurrentUser();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("message");
            }
        }
    }
}
