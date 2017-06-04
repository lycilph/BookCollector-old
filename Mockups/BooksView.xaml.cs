using System.Windows;

namespace Mockups
{
    public partial class BooksView
    {
        public BooksView()
        {
            InitializeComponent();
        }

        private void ImportClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowImportView();
        }

        private void ChangeCollectionClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowCollectionsView();
        }

        private void GoodreadsClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowWebView();
        }
    }
}
