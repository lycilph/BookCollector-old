using BookCollector.Framework.Logging;
using BookCollector.Framework.MVVM;
using ReactiveUI;
using IScreen = BookCollector.Framework.MVVM.IScreen;

namespace BookCollector.Screens.Main
{
    public class MainViewModel : ScreenBase, IMainViewModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        private IScreen _MainContent;
        public IScreen MainContent
        {
            get { return _MainContent; }
            set { this.RaiseAndSetIfChanged(ref _MainContent, value); }
        }

        private IScreen _ExtraContent;
        public IScreen ExtraContent
        {
            get { return _ExtraContent; }
            set { this.RaiseAndSetIfChanged(ref _ExtraContent, value); }
        }

        private IScreen _MenuContent;
        public IScreen MenuContent
        {
            get { return _MenuContent; }
            set { this.RaiseAndSetIfChanged(ref _MenuContent, value); }
        }

        private bool _IsMenuOpen;
        public bool IsMenuOpen
        {
            get { return _IsMenuOpen; }
            set { this.RaiseAndSetIfChanged(ref _IsMenuOpen, value); }
        }

        public MainViewModel()
        {
            DisplayName = ScreenNames.MainName;
            IsMenuOpen = false;
        }

        public void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }

        public void Show(IScreen content)
        {
            // Deactivate old content
            MainContent?.Deactivate();

            // Activate new content
            content?.Activate();

            // Show new content
            MainContent = content;
            DisplayName = MainContent.DisplayName;

            if (MainContent is IMainScreen)
            {
                var main_screen_content = MainContent as IMainScreen;

                // If screen have extra content, show this as well else hide it
                ExtraContent = main_screen_content.ExtraContent;
                // If screen have menu content, show this as well else hide it
                MenuContent = main_screen_content.MenuContent;
            }
        }
    }
}
