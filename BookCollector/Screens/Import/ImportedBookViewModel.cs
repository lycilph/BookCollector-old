using BookCollector.Data.Import;
using Core;
using Core.Utility;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ImportedBookViewModel : ItemViewModel<ImportedBook>
    {
        public string Title { get { return Obj.Title; } }
        public string Authors { get { return string.Join(", ", Obj.Authors); } }
        public string Shelves { get { return string.Join(", ", Obj.Shelves); } }
        public int SimilarityScore { get { return Obj.Similarity.Score; } }
        public string SimilarityText { get { return Obj.Similarity.Text; } }
        public string SimilarityTextShort { get { return Obj.Similarity.TextShort; } }

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set { this.RaiseAndSetIfChanged(ref _Selected, value); }
        }

        public ImportedBookViewModel(ImportedBook obj) : base(obj) { }
    }
}
