using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace BookCollector.Shell
{
    public class DialogService : IDialogService
    {
        public Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            var metro_window = GetMetroWindow();
            return metro_window.Invoke(() => metro_window.ShowMessageAsync(title, message, style, settings));
        }

        public Task ShowCustomDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            var metro_window = GetMetroWindow();
            return metro_window.Invoke(() => metro_window.ShowMetroDialogAsync(dialog, settings));
        }

        public Task HideCustomDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            var metro_window = GetMetroWindow();
            return metro_window.Invoke(() => metro_window.HideMetroDialogAsync(dialog, settings));
        }

        private MetroWindow GetMetroWindow()
        {
            var window = Application.Current.MainWindow as MetroWindow;
            if (window == null)
                throw new InvalidOperationException("Main window must be a MetroWindow");
            return window;
        }
    }
}
