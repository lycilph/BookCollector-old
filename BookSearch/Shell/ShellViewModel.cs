using BookSearch.Screens.Main;
using NLog;
using Panda.ApplicationCore.Shell;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using LogManager = NLog.LogManager;
using IScreen = Caliburn.Micro.IScreen;

namespace BookSearch.Shell
{
    [Export(typeof(IShell))]
    public sealed class ShellViewModel : ConductorShellBase<IScreen>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Stack<IScreen> screens = new Stack<IScreen>();

        private string _MainStatusText;
        public string MainStatusText
        {
            get { return _MainStatusText; }
            set
            {
                this.RaiseAndSetIfChanged(ref _MainStatusText, value);
                logger.Trace("MainStatusText: " + value);
            }
        }

        private string _AuxiliaryStatusText;
        public string AuxiliaryStatusText
        {
            get { return _AuxiliaryStatusText; }
            set
            {
                this.RaiseAndSetIfChanged(ref _AuxiliaryStatusText, value);
                logger.Trace("AuxiliaryStatusText: " + value);
            }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        public ShellViewModel()
        {
            DisplayName = "BookSearch";
            Show(new MainViewModel(this));
        }

        protected override void ChangeActiveItem(IScreen new_item, bool close_previous)
        {
            logger.Trace("Changing to screen: " + new_item.DisplayName);
            MainStatusText = string.Empty;
            AuxiliaryStatusText = string.Empty;
            base.ChangeActiveItem(new_item, close_previous);
        }

        public void Back()
        {
            screens.Pop();
            ActivateItem(screens.Peek());
        }

        public void Show(IScreen screen)
        {
            screens.Push(screen);
            ActivateItem(screen);
        }
    }
}
