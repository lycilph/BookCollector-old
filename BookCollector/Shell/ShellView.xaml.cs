using System.ComponentModel;
using System.Windows;
using BookCollector.Domain;
using BookCollector.Framework.Messaging;

namespace BookCollector.Shell
{
    public partial class ShellView : IShellView
    {
        private IEventAggregator event_aggregator;

        public ShellView(IEventAggregator event_aggregator, IShellViewModel shell_view_model)
        {
            this.event_aggregator = event_aggregator;

            InitializeComponent();

            DataContext = shell_view_model;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            event_aggregator.Publish(ApplicationMessage.ShellLoaded());
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            event_aggregator.Publish(ApplicationMessage.ShellClosing());
        }
    }
}
