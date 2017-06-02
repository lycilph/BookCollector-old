using System.Windows;

namespace Mockups
{
    public partial class BooksView
    {
        public BooksView()
        {
            InitializeComponent();
        }

        private void ChangeCollectionClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowCollectionsView();
        }

        private void ImportClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowImportView();
        }

        private void GoodreadsClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowWebView();
        }
    }
}
