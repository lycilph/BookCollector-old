using BookCollector.Data;
using Core;
using Core.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class AddShelfDialogViewModel : DialogScreenBase
    {
        private Shelf shelf;

        public string Name
        {
            get { return shelf.Name; }
            set
            {
                if (value == shelf.Name)
                    return;
                this.RaisePropertyChanging();
                shelf.Name = value;
                this.RaisePropertyChanged();
            }
        }

        private ReactiveCommand _ClearCommand;
        public ReactiveCommand ClearCommand
        {
            get { return _ClearCommand; }
            set { this.RaiseAndSetIfChanged(ref _ClearCommand, value); }
        }

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

        public AddShelfDialogViewModel(Shelf shelf)
        {
            DisplayName = "Add shelf";
            this.shelf = shelf;

            ClearCommand = ReactiveCommand.Create(() => Name = string.Empty);

            var can_accept = this.WhenAny(x => x.Name, x => !string.IsNullOrWhiteSpace(x.Value));
            OkCommand = ReactiveCommand.Create(() => SetResult(MessageDialogResult.Affirmative), can_accept);

            CancelCommand = ReactiveCommand.Create(() => SetResult(MessageDialogResult.Negative));
        }
    }
}
