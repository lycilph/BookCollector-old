using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ShelfViewModel : ItemViewModel<Shelf>
    {
        public string Name { get { return obj.Name; } }

        private bool _Enabled = false;
        public bool Enabled
        {
            get { return _Enabled; }
            set { this.RaiseAndSetIfChanged(ref _Enabled, value); }
        }

        public ShelfViewModel(Shelf obj) : base(obj) { }
    }
}
