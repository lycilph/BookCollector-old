using System;
using System.Windows;

namespace Mockups
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowBooksView()
        {
            ShellContent.Content = new MainView();

            CollectionNameButton.Visibility = Visibility.Visible;
        }

        public void ShowWebView()
        {
            ShellContent.Content = new WebView();

            CollectionNameButton.Visibility = Visibility.Hidden;
        }

        internal void ShowImportView()
        {
            var main_view = ShellContent.Content as MainView;
            if (main_view == null)
                throw new InvalidOperationException("Shell content must be a MainView");

            main_view.MainContent.Content = new ImportView();
            main_view.SearchBox.Visibility = Visibility.Hidden;
            main_view.MenuToggleButton.Visibility = Visibility.Hidden;
            main_view.ScreenTitle.Text = "Import";

            CollectionNameButton.Visibility = Visibility.Hidden;
        }

        internal void ShowCollectionsView()
        {
            var main_view = ShellContent.Content as MainView;
            if (main_view == null)
                throw new InvalidOperationException("Shell content must be a MainView");

            main_view.MainContent.Content = new CollectionsView();
            main_view.SearchBox.Visibility = Visibility.Hidden;
            main_view.MenuToggleButton.Visibility = Visibility.Hidden;
            main_view.ScreenTitle.Text = "Collections";

            CollectionNameButton.Visibility = Visibility.Hidden;
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }

        private void ChangeCollectionClick(object sender, RoutedEventArgs e)
        {
            ShowCollectionsView();
        }
    }
}
