using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace BookCollector.Framework.Dialog
{
    public interface IDialogService
    {
        Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null);
        Task ShowCustomDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null);
        Task HideCustomDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null);
    }
}
