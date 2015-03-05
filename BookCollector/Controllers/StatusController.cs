using System;
using System.ComponentModel.Composition;
using System.Windows;
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
        private readonly StatusBarTextItemViewModel main_status_text = new StatusBarTextItemViewModel();
        private readonly StatusBarTextItemViewModel aux_status_text = new StatusBarTextItemViewModel(GridLength.Auto);
        private readonly StatusBarProgressItemViewModel progress_status = new StatusBarProgressItemViewModel(true, new GridLength(200));

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
        public StatusController(IShell shell)
        {
            shell.StatusBar.Add(main_status_text, aux_status_text, progress_status);
        }

        private void ClearStatusText()
        {
            MainStatusText = string.Empty;
            AuxiliaryStatusText = string.Empty;
        }

        public void Handle(ShellMessage message)
        {
            switch (message)
            {
                case ShellMessage.ActiveItemChanged:
                    ClearStatusText();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("message");
            }
        }
    }
}
