using Core;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class EditShelfDialogViewModel : DialogScreenBase
    {
        private ReactiveCommand _OkCommand;
        public ReactiveCommand OkCommand
        {
            get { return _OkCommand; }
            set { this.RaiseAndSetIfChanged(ref _OkCommand, value); }
        }

        private ReactiveCommand _CancelCommand;
        public ReactiveCommand CancelCommand
        {
            get { return _CancelCommand; }
            set { this.RaiseAndSetIfChanged(ref _CancelCommand, value); }
        }

        public EditShelfDialogViewModel()
        {
            DisplayName = "Edit shelf";

            OkCommand = ReactiveCommand.Create(() => SetResult(MessageDialogResult.Affirmative));
            CancelCommand = ReactiveCommand.Create(() => SetResult(MessageDialogResult.Negative));
        }
    }
}
