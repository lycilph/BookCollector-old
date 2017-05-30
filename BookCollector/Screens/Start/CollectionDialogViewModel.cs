using System;
using ReactiveUI;

namespace BookCollector.Screens.Start
{
    public class CollectionDialogViewModel : ReactiveObject
    {
        private CollectionDescriptionViewModel collection_description;

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
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
        
        public CollectionDialogViewModel(CollectionDescriptionViewModel collection_description, Action close_handler)
        {
            this.collection_description = collection_description;

            Name = collection_description.Name;
            Description = collection_description.Description;

            OkCommand = ReactiveCommand.Create(() =>
            {
                collection_description.Name = Name;
                collection_description.Description = Description;

                close_handler();
            });
            CancelCommand = ReactiveCommand.Create(close_handler);
        }
    }
}
