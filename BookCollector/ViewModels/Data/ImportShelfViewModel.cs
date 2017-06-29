using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Data
{
    public class ImportShelfViewModel : ItemViewModel<Shelf>
    {
        public string Name { get { return Obj.Name; } }

        private bool _Enabled = false;

        public bool Enabled
        {
            get { return _Enabled; }
            set { this.RaiseAndSetIfChanged(ref _Enabled, value); }
        }

        public ImportShelfViewModel(Shelf obj) : base(obj) { }
    }
}
