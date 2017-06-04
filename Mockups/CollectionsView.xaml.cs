using System;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Mockups
{
    public partial class CollectionsView
    {
        private ResourceDictionary dialog_dictionary = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") };

        public CollectionsView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            var dialog_settings = new MetroDialogSettings
            {
                SuppressDefaultResources = true,
                CustomResourceDictionary = dialog_dictionary,
            };

            var dialog = new CustomDialog(Application.Current.MainWindow as MetroWindow, dialog_settings);
            var view = new CollectionDialogView(() => DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog));

            dialog.Title = "Add Collection";
            dialog.Content = view;

            DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog, dialog_settings);
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            var dialog_settings = new MetroDialogSettings
            {
                SuppressDefaultResources = true,
                CustomResourceDictionary = dialog_dictionary,
            };

            var dialog = new CustomDialog(Application.Current.MainWindow as MetroWindow, dialog_settings);
            var view = new CollectionDialogView(() => DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog));
            view.CollectionNameTextBox.Text = "Existing Name";
            view.CollectionDescriptionTextBox.Text = "Existing Description";

            dialog.Title = "Edit Collection";
            dialog.Content = view;

            DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog, dialog_settings);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var dialog_settings = new MetroDialogSettings
            {
                SuppressDefaultResources = true,
                CustomResourceDictionary = dialog_dictionary,
                NegativeButtonText = "CANCEL"
            };

            DialogCoordinator.Instance.ShowMessageAsync(this, "Warning", "Are you sure you want to delete the collection?", MessageDialogStyle.AffirmativeAndNegative, dialog_settings);
        }

        private void ContinueClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowBooksView();
        }
    }
}
