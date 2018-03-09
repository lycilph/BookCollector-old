using BookCollector.Data;
using Core.Utility;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class DescriptionViewModel : ItemViewModel<Description>
    {
        public string Name { get { return Obj.Name; } }
        public string Summary { get { return Obj.Summary; } }
        public string Filename { get { return Obj.Filename; } }
        public string LastModifiedDate {  get { return Obj.LastModifiedDate.ToShortDateString(); } }
        public string Details { get; set; }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsSelected, value); }
        }

        public DescriptionViewModel(Collection collection) : base(collection.Description)
        {
            Details = $"Collection consists of {collection.Books.Count} books in {collection.Shelves.Count} shelves";
        }
    }
}
