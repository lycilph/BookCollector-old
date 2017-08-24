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

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsSelected, value); }
        }

        public DescriptionViewModel(Description obj) : base(obj)
        {
        }
    }
}
