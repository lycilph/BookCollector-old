using System;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Core.Dialogs
{
    public interface IDialogService
    {
        void ShowDialogAsync(IDialogScreen vm, Action<MessageDialogResult> handler = null, MetroDialogSettings settings = null);
        Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null);
        FileDialogResult ShowFileDialog(string title, string ext, string filter);
    }
}