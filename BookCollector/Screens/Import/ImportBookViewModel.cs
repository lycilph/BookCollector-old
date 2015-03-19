using BookCollector.Api.ImportProvider;
using Panda.ApplicationCore.Utilities;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ImportBookViewModel : ItemViewModelBase<ImportedBook>
    {
        public string Title { get { return AssociatedObject.Title; } }
        public string Authors { get { return string.Join(", ", AssociatedObject.Authors); } }
        public string ISBN10 { get { return AssociatedObject.ISBN10; } }
        public string ISBN13 { get { return AssociatedObject.ISBN13; } }
        public string Shelf { get { return AssociatedObject.Shelf; } }
        public string Source { get { return AssociatedObject.Source; } }

        public bool IsDuplicate { get; set; }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsSelected, value); }
        }

        public ImportBookViewModel(ImportedBook obj, bool is_duplicate) : base(obj)
        {
            IsDuplicate = is_duplicate;
            IsSelected = !IsDuplicate;
        }
    }
}
