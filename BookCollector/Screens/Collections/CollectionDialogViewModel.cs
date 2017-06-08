using System;
using System.Reactive.Linq;
using BookCollector.Framework.Mapping;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class CollectionDialogViewModel : ReactiveObject
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
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

        public CollectionDialogViewModel(DescriptionViewModel description, Action<MessageDialogResult> close_handler)
        {
            Mapper.Map(description, this);

            var can_accept = this.WhenAnyValue(x => x.Name, x => x.Text)
                                 .Select(p => !string.IsNullOrWhiteSpace(p.Item1) && !string.IsNullOrWhiteSpace(p.Item2));

            OkCommand = ReactiveCommand.Create(() =>
            {
                Mapper.Map(this, description);
                close_handler(MessageDialogResult.Affirmative);
            }, can_accept);
            CancelCommand = ReactiveCommand.Create(() => close_handler(MessageDialogResult.Negative));
        }
    }
}
