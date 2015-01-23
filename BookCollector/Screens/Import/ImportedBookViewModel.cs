using BookCollector.Model;
using Framework.Core.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ImportedBookViewModel : ItemViewModelBase<ImportedBook>
    {
        public string Title { get { return AssociatedObject.Book.Title; } }
        public bool IsDuplicate { get; set; }
        public string ImportSource { get; set; }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsSelected, value); }
        }

        public ImportedBookViewModel(ImportedBook obj) : base(obj) { }
    }
}
