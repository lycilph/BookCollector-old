using System.ComponentModel;
using System.Windows;
using BookCollector.Framework.MVVM;

namespace BookCollector.Shell
{
    public partial class ShellView
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IViewAware)
            {
                (DataContext as IViewAware).OnViewLoaded();
            }
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is IViewAware)
            {
                (DataContext as IViewAware).OnViewClosing();
            }
        }
    }
}
