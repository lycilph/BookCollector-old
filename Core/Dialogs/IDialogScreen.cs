using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Core.Dialogs
{
    public interface IDialogScreen
    {
        string DisplayName { get; set; }
        Task<MessageDialogResult> DialogResultTask { get; }
    }
}
