using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Core
{
    public interface IDialogScreen
    {
        string DisplayName { get; set; }
        Task<MessageDialogResult> DialogResultTask { get; }
    }
}
