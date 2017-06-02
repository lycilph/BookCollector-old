using ReactiveUI;

namespace Mockups
{
    public class ImportedBookViewModel : ReactiveObject
    {
        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set { this.RaiseAndSetIfChanged(ref _Selected, value); }
        }

        public string Name { get; set; }
        public string Authors { get; set; }
        public string Similarity { get; set; }
    }
}
