using System.Collections.Generic;
using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Common
{
    public class ImportedBookViewModel : ItemViewModel<Book>
    {
        public string Title { get { return obj.Title; } }
        public string Authors { get { return string.Join(", ", obj.Authors); } }
        public List<Shelf> Shelves { get { return obj.Shelves; } }

        private int _Similarity;
        public int Similarity
        {
            get { return _Similarity; }
            set { this.RaiseAndSetIfChanged(ref _Similarity, value); }
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

        public ImportedBookViewModel(Book obj) : base(obj) { }
    }
}
