using ReactiveUI;

namespace BookCollector.Framework.MVVM
{
    public class MainScreenBase : ScreenBase, IMainScreen
    {
        private bool _ShowCollectionCommand;
        public bool ShowCollectionCommand
        {
            get { return _ShowCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowCollectionCommand, value); }
        }

        private IScreen _MenuContent;
        public IScreen MenuContent
        {
            get { return _MenuContent; }
            set { this.RaiseAndSetIfChanged(ref _MenuContent, value); }
        }

        private IScreen _ExtraContent;
        public IScreen ExtraContent
        {
            get { return _ExtraContent; }
            set { this.RaiseAndSetIfChanged(ref _ExtraContent, value); }
        }
    }
}
