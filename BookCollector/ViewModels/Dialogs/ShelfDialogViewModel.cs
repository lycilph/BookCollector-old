using System;
using BookCollector.ViewModels.Common;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.ViewModels.Dialogs
{
    public class ShelfDialogViewModel : ReactiveObject
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
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

        public ShelfDialogViewModel(ShelfViewModel shelf, Action<MessageDialogResult> close_handler)
        {
            Name = shelf.Name;

            var can_accept = this.WhenAny(x => x.Name, x => !string.IsNullOrWhiteSpace(x.Value));

            OkCommand = ReactiveCommand.Create(() =>
            {
                shelf.Name = Name;
                close_handler(MessageDialogResult.Affirmative);
            }, can_accept);
            CancelCommand = ReactiveCommand.Create(() => close_handler(MessageDialogResult.Negative));
        }
    }
}
