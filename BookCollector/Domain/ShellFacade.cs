using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Shell;

namespace BookCollector.Domain
{
    public class ShellFacade : IShellFacade
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IShellViewModel shell_view_model;
        private IShellView shell_view;
        private IWindowCommand collection_command;

        public ShellFacade(IEventAggregator event_aggregator, IShellViewModel shell_view_model, IShellView shell_view)
        {
            this.event_aggregator = event_aggregator;
            this.shell_view_model = shell_view_model;
            this.shell_view = shell_view;
        }

        public void Initialize()
        {
            log.Info("Initializing shell facade");

            collection_command = new WindowCommand("Import", () => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.ImportScreenDisplayName)));
            shell_view_model.RightShellCommands.Add(collection_command);

            // Add flyouts

            // Show the shell window
            shell_view.Show();
        }

        public void SetCollectionCommandVisibility(bool is_visible)
        {
            collection_command.IsVisible = is_visible;
        }

        public void SetFullscreenState(bool is_fullscreen)
        {
            shell_view_model.IsFullscreen = is_fullscreen;
        }

        public void ShowMainContent(IScreen content)
        {
            shell_view_model.ShowMainContent(content);
        }

        public void ShowMenuContent(IScreen content)
        {
            shell_view_model.ShowMenuContent(content);
        }

        public void ShowHeaderContent(IScreen content)
        {
            shell_view_model.ShowHeaderContent(content);
        }
    }
}
