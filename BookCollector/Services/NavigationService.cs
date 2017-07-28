using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Screens.Shell;
using Core;
using NLog;

namespace BookCollector.Services
{
    public class NavigationService : INavigationService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IShellViewModel shell;
        private List<IScreen> screens;
        private Dictionary<Type, ScreenConfiguration> configurations = new Dictionary<Type, ScreenConfiguration>();

        public NavigationService(IShellViewModel shell, IEnumerable<IScreen> screens)
        {
            this.shell = shell;
            this.screens = screens.ToList();

            Initialize();
        }

        private void Initialize()
        {
            logger.Trace("Initialize");

            // Go through screens and register flyouts
            foreach (var screen in screens)
            {
                if (screen is IFlyout flyout)
                {
                    logger.Trace($"Registering {screen.GetType().Name} as IFlyout");
                    shell.ShellFlyouts.Add(flyout);
                }
            }
        }

        public void Register(Type screen_type, ShellScreenPosition position = ShellScreenPosition.MainContent, bool show_collection_command = true, bool is_fullscreen = false)
        {
            var configuration = new ScreenConfiguration
            {
                position = position,
                show_collection_command = show_collection_command,
                is_fullscreen = is_fullscreen
            };
            Register(screen_type, configuration);
        }

        private void Register(Type screen_type, ScreenConfiguration configuration)
        {
            if (configurations.ContainsKey(screen_type))
                return;

            // Try to find corresponding screen
            var screen = screens.Single(s => screen_type.IsAssignableFrom(s.GetType()));
            configuration.screen = screen;

            // Is this a flyout
            if (screen is IFlyout flyout)
                configuration.is_flyout = true;

            configurations[screen_type] = configuration;
        }

        public void NavigateTo(Type screen_type)
        {
            logger.Trace($"Navigating to {screen_type.Name}");

            if (!configurations.TryGetValue(screen_type, out ScreenConfiguration configuration))
                throw new InvalidOperationException($"Could not find ScreenConfiguration for {screen_type.Name}");

            // Handle flyouts
            if (configuration.is_flyout)
            {
                if (!(configuration.screen is IFlyout flyout))
                    throw new InvalidOperationException($"Expected {screen_type.Name} to be of type IFlyout");
                flyout.Toggle();
            }
            // Handle main, header and menu content
            else if (configuration.position == ShellScreenPosition.MainContent)
            {
                shell.CollectionCommand.IsVisible = configuration.show_collection_command;
                shell.Show(configuration.screen, configuration.position, configuration.is_fullscreen);
            }
            else
                shell.Show(configuration.screen, configuration.position);
        }

        private class ScreenConfiguration
        {
            public IScreen screen;
            public ShellScreenPosition position = ShellScreenPosition.MainContent;
            public bool show_collection_command = true;
            public bool is_fullscreen = false;
            public bool is_flyout = false;
        }
    }
}
