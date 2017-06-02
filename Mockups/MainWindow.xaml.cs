using System;
using System.Windows;
using System.Windows.Controls;

namespace Mockups
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowCollectionsView()
        {
            ShellContent.Content = new CollectionsView();
            SearchBox.Visibility = Visibility.Hidden;
            CollectionNameButton.Visibility = Visibility.Hidden;
            ScreenTitle.Text = "Collections";
            MenuToggleButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
        }

        public void ShowBooksView()
        {
            ShellContent.Content = new BooksView();
            SearchBox.Visibility = Visibility.Visible;
            CollectionNameButton.Visibility = Visibility.Visible;
            ScreenTitle.Text = "Books";
            MenuToggleButton.IsEnabled = true;
            SettingsButton.IsEnabled = true;
        }

        internal void ShowWebView()
        {
            ShellContent.Content = new WebView();
            SearchBox.Visibility = Visibility.Hidden;
            CollectionNameButton.Visibility = Visibility.Hidden;
            ScreenTitle.Text = "Web";
            MenuToggleButton.IsEnabled = false;
            SettingsButton.IsEnabled = true;
        }

        public void ShowImportView()
        {
            ShellContent.Content = new ImportView();
            SearchBox.Visibility = Visibility.Hidden;
            CollectionNameButton.Visibility = Visibility.Hidden;
            ScreenTitle.Text = "Import";
            MenuToggleButton.IsEnabled = false;
            SettingsButton.IsEnabled = true;
        }

        private void ChangeCollectionClick(object sender, RoutedEventArgs e)
        {
            ShowCollectionsView();
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }

        private void ShelfChangedClick(object sender, SelectionChangedEventArgs e)
        {
            if (MenuToggleButton != null)
                MenuToggleButton.IsChecked = !MenuToggleButton.IsChecked;
        }
    }
}
