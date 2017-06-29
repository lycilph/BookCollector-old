using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Data
{
    public class ImportBookViewModel : ItemViewModel<Book>
    {
        public string Title { get { return Obj.Title; } }
        public string Authors { get { return string.Join(", ", Obj.Authors); } }

        private int _SimilarityScore;
        public int SimilarityScore
        {
            get { return _SimilarityScore; }
            set { this.RaiseAndSetIfChanged(ref _SimilarityScore, value); }
        }

        private string _SimilarityText;
        public string SimilarityText
        {
            get { return _SimilarityText; }
            set { this.RaiseAndSetIfChanged(ref _SimilarityText, value); }
        }

        private string _SimilarityTextShort;
        public string SimilarityTextShort
        {
            get { return _SimilarityTextShort; }
            set { this.RaiseAndSetIfChanged(ref _SimilarityTextShort, value); }
        }

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set { this.RaiseAndSetIfChanged(ref _Selected, value); }
        }

        public ImportBookViewModel(Book obj) : base(obj) { }
    }
}
