using System.ComponentModel;
using System.Windows;

namespace BookCollector.Shell
{
    public partial class ShellView : IShellView
    {
        public ShellView(IShellViewModel shell_view_model)
        {
            InitializeComponent();

            DataContext = shell_view_model;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //if (DataContext is IViewAware)
            //{
            //    (DataContext as IViewAware).OnViewLoaded();
            //}
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            //if (DataContext is IViewAware)
            //{
            //    (DataContext as IViewAware).OnViewClosing();
            //}
        }
    }
}
