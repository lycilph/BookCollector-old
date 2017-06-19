using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Common
{
    public class ImportedShelfViewModel : ItemViewModel<Shelf>
    {
        public string Name { get { return obj.Name; } }

        private bool _Enabled = false;
        public bool Enabled
        {
            get { return _Enabled; }
            set { this.RaiseAndSetIfChanged(ref _Enabled, value); }
        }

        public ImportedShelfViewModel(Shelf obj) : base(obj) { }
    }
}
