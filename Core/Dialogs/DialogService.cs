using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Core.Application;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace Core.Dialogs
{
    public class DialogService : IDialogService
    {
        public Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            var metro_window = GetMetroWindow();
            return metro_window.Invoke(() => metro_window.ShowMessageAsync(title, message, style, settings));
        }

        public FileDialogResult ShowFileDialog(string title, string ext, string filter)
        {
            var ofd = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = Assembly.GetExecutingAssembly().Location,
                DefaultExt = ext,
                Filter = filter
            };

            var result = (ofd.ShowDialog() == true ? MessageDialogResult.Affirmative : MessageDialogResult.Negative);
            return new FileDialogResult(result, ofd.FileName);
        }

        public async void ShowDialogAsync(IDialogScreen vm, Action<MessageDialogResult> handler = null, MetroDialogSettings settings = null)
        {
            if (settings == null)
                settings = new MetroDialogSettings();

            // Override default styles (see https://github.com/ButchersBoy/MaterialDesignInXamlToolkit/wiki/MahApps.Metro-integration)
            settings.SuppressDefaultResources = true;
            settings.CustomResourceDictionary = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
            };

            var dialog = new CustomDialog(settings) { Title = vm.DisplayName };
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
