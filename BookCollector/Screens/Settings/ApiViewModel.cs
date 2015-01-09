using BookCollector.Apis;
using Framework.Core.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Settings
{
    public class ApiViewModel : ItemViewModelBase<IApi>
    {
        public string DisplayName { get { return AssociatedObject.Name; } }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        public ApiViewModel(IApi obj) : base(obj) { }
    }
}
