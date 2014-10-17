using System;
using System.Windows;
using System.Windows.Input;
using Framework.Dialogs;
using MahApps.Metro.Controls;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace BookCollector.Main
{
    public partial class BookDetailsView
    {
        private MetroWindow window;

        public BookDetailsView()
        {
            InitializeComponent();
        }

        private void BookDetailsView_OnLoaded(object sender, RoutedEventArgs e)
        {
            window = VisualTreeHelperEx.FindAncestorByType<MetroWindow>(this);
            if (window == null)
                throw new Exception("This dialog must be contained in a MetroWindow");

            window.PreviewMouseLeftButtonUp += WindowOnPreviewMouseLeftButtonUp;
        }

        private void WindowOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouse_button_event_args)
        {
            var host = VisualTreeHelperEx.FindAncestorByType<HostDialog>(this);
            if (host == null)
                throw new Exception("This dialog must be hosted by a HostDialog");

            var p = mouse_button_event_args.GetPosition(host);
            if (p.Y > 0 && p.Y < host.ActualHeight)
                return;

            var vm = DataContext as BookDetailsViewModel;
            if (vm == null) return;
            vm.Close();
        }

        private void BookDetailsView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            window.PreviewMouseLeftButtonUp -= WindowOnPreviewMouseLeftButtonUp;
            window = null;
        }
    }
}
