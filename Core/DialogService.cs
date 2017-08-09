using System;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Core
{
    public class DialogService : IDialogService
    {
        public Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            var metro_window = GetMetroWindow();
            return metro_window.Invoke(() => metro_window.ShowMessageAsync(title, message, style, settings));
        }

        public async void ShowDialogAsync(IDialogScreen vm, Action<MessageDialogResult> handler = null, MetroDialogSettings settings = null)
        {
            var dialog = new CustomDialog { Title = vm.DisplayName };
            dialog.Content = ViewManager.CreateAndBindViewForModel(vm);

            await ShowCustomDialogAsync(dialog, settings);
            var result = await vm.DialogResultTask;
            await HideCustomDialogAsync(dialog, settings);

            handler?.Invoke(result);
        }

        private Task ShowCustomDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            var metro_window = GetMetroWindow();
            return metro_window.Invoke(() => metro_window.ShowMetroDialogAsync(dialog, settings));
        }

        private Task HideCustomDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            var metro_window = GetMetroWindow();
            return metro_window.Invoke(() => metro_window.HideMetroDialogAsync(dialog, settings));
        }

        private MetroWindow GetMetroWindow()
        {
            var window = System.Windows.Application.Current.MainWindow as MetroWindow;
            if (window == null)
                throw new InvalidOperationException("Main window must be a MetroWindow");
            return window;
        }
    }
}
