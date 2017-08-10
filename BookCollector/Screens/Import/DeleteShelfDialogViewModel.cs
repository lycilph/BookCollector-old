using System.Linq;
using BookCollector.Data;
using Core;
using Core.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class DeleteShelfDialogViewModel : DialogScreenBase
    {
        private ReactiveList<Shelf> _Shelves;
        public ReactiveList<Shelf> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private Shelf _SelectedShelf;
        public Shelf SelectedShelf
        {
            get { return _SelectedShelf; }
            set { this.RaiseAndSetIfChanged(ref _SelectedShelf, value); }
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

        public DeleteShelfDialogViewModel(ReactiveList<Shelf> shelves)
        {
            DisplayName = "Delete shelf";
            Shelves = shelves;
            SelectedShelf = Shelves.FirstOrDefault();

            var can_delete = this.WhenAny(x => x.SelectedShelf, x => x.Value != null && !x.Value.IsDefault);

            OkCommand = ReactiveCommand.Create(() => SetResult(MessageDialogResult.Affirmative), can_delete);
            CancelCommand = ReactiveCommand.Create(() => SetResult(MessageDialogResult.Negative));
        }
    }
}
