using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace Core
{
    public class DialogScreenBase : ReactiveObject, IDialogScreen
    {
        protected TaskCompletionSource<MessageDialogResult> tcs = new TaskCompletionSource<MessageDialogResult>();

        public Task<MessageDialogResult> DialogResultTask { get { return tcs.Task; } }

        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        public void SetResult(MessageDialogResult result)
        {
            tcs.SetResult(result);
        }
    }
}
