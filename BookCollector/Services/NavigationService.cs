using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MVVM;
using BookCollector.Shell;

namespace BookCollector.Services
{
    // This should handle all screen transitions
    public class NavigationService : INavigationService
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IShellFacade shell;
        private Dictionary<string, IScreen> screens;
        private IConfigurationService configuration_service;

        public NavigationService(IShellFacade shell, IScreen[] all_screens, IConfigurationService configuration_service)
        {
            this.shell = shell;
            this.configuration_service = configuration_service;
            screens = all_screens.ToDictionary(s => s.DisplayName);
        }

        public void Initialize()
        {
            // Add all flyouts here
            foreach (var configuration in configuration_service.GetFlyouts())
            {
                var flyout = screens[configuration.MainContent] as IFlyout;
                if (flyout == null)
                    throw new ArgumentException($"{configuration.MainContent} must inherit from FlyoutBase");
                shell.AddFlyout(flyout);
            }
        }

        public void NavigateTo(string screen_name)
        {
            log.Info($"Navigating to {screen_name}");

            var configuration = configuration_service.Get(screen_name);

            if (configuration.IsFlyout)
            {
                var flyout = screens[screen_name] as IFlyout;
                if (flyout == null)
                    throw new ArgumentException($"{screen_name} must inherit from FlyoutBase");
                flyout.Toggle();
                return;
            }

            shell.SetCollectionCommandVisibility(configuration.ShowCollectionCommand);
            shell.SetFullscreenState(configuration.IsFullscreen);

            var main_content = screens[configuration.MainContent];
            shell.ShowMainContent(main_content);

            screens.TryGetValue(configuration.HeaderContent, out IScreen header_content);
            shell.ShowHeaderContent(header_content);

            screens.TryGetValue(configuration.MenuContent, out IScreen menu_content);
            shell.ShowMenuContent(menu_content);
        }
    }
}
