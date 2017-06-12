using System.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class ShelvesViewModel : ScreenBase
    {
        private IApplicationModel application_model;

        private ReactiveList<ShelfViewModel> _Shelves;
        public ReactiveList<ShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private ShelfViewModel _SelectedShelf;
        public ShelfViewModel SelectedShelf
        {
            get { return _SelectedShelf; }
            set { this.RaiseAndSetIfChanged(ref _SelectedShelf, value); }
        }

        private ReactiveCommand _AddCommand;
        public ReactiveCommand AddCommand
        {
            get { return _AddCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddCommand, value); }
        }

        public ShelvesViewModel(IEventAggregator event_aggregator, IApplicationModel application_model)
        {
            this.application_model = application_model;
            DisplayName = ScreenNames.ShelvesName;

            AddCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.ToggleMainMenuMessage()));
        }

        public override void Activate()
        {
            SelectedShelf = Shelves.Single(s => s.Name == application_model.CurrentShelf.Name);
        }

        public override void Deactivate()
        {
            application_model.CurrentShelf = SelectedShelf.Unwrap();
        }
    }
}
