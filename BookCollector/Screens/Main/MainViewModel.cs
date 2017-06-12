using System;
using System.Reactive.Linq;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MVVM;
using ReactiveUI;
using IScreen = BookCollector.Framework.MVVM.IScreen;

namespace BookCollector.Screens.Main
{
    public class MainViewModel : ScreenBase, IMainViewModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        private IMainScreen _MainContent;
        public IMainScreen MainContent
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

            this.WhenAnyValue(x => x.IsMenuOpen)
                .Subscribe(open => 
                {
                    if (open)
                        MenuContent?.Activate();
                    else
                        MenuContent?.Deactivate();
                });

            this.WhenAnyValue(x => x.MainContent)
                .Where(content => content != null)
                .Subscribe(content =>
                {
                    // Update title when content changes
                    DisplayName = content.DisplayName;
                    // Set extra content and menu content
                    ExtraContent = content.ExtraContent;
                    MenuContent = content.MenuContent;
                });
        }

        public void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }

        public void Show(IMainScreen content)
        {
            if (MainContent == content)
                return;

            // Deactivate old content
            MainContent?.Deactivate();
            // Activate new content
            content?.Activate();

            // Show new content
            MainContent = content;
        }
    }
}
