using BookCollector.Model;
using BookCollector.Services.Repository;
using Framework.Core.MVVM;
using ReactiveUI;

namespace BookCollector.Import
{
    public class ImportedBookViewModel : ItemViewModelBase<ImportedBook>
    {
        private readonly Book duplicate;

        public string Title { get { return AssociatedObject.Book.Title; } }
        public bool IsDuplicate { get { return duplicate != null; } }
        public string ImportSource { get { return (duplicate == null ? "" : duplicate.ImportSource); } }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsSelected, value); }
        }

        public ImportedBookViewModel(ImportedBook obj, Book duplicate) : base(obj)
        {
            this.duplicate = duplicate;
        }
    }
}
