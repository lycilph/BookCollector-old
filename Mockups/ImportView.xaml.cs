using System.Collections.ObjectModel;
using System.Windows;

namespace Mockups
{
    public partial class ImportView
    {
        public ObservableCollection<ImportedBookViewModel> ImportedBooks
        {
            get { return (ObservableCollection<ImportedBookViewModel>)GetValue(ImportedBooksProperty); }
            set { SetValue(ImportedBooksProperty, value); }
        }
        public static readonly DependencyProperty ImportedBooksProperty = 
            DependencyProperty.Register("ImportedBooks", typeof(ObservableCollection<ImportedBookViewModel>), typeof(ImportView), new PropertyMetadata(null));

        public ImportView()
        {
            InitializeComponent();
            DataContext = this;

            ImportedBooks = new ObservableCollection<ImportedBookViewModel>
            {
                new ImportedBookViewModel { Name = "Book 1", Authors = "Author 1", Similarity = "Similarity: 25" },
                new ImportedBookViewModel { Name = "Book 2", Authors = "Author 2", Similarity = "Similarity: 50" },
                new ImportedBookViewModel { Name = "Book 3", Authors = "Author 3", Similarity = "Similarity: 75" },
                new ImportedBookViewModel { Name = "Book 4", Authors = "Author 1", Similarity = "Similarity: 100" },
                new ImportedBookViewModel { Name = "Book 5", Authors = "Author 2", Similarity = "Similarity: 0" },
                new ImportedBookViewModel { Name = "Book 6", Authors = "Author 3", Similarity = "Similarity: 25" }
            };
        }

        private void ContinueClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowBooksView();
        }

        private void SelectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var b in ImportedBooks)
                b.Selected = true;
        }

        private void DeselectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var b in ImportedBooks)
                b.Selected = false;
        }
    }
}
