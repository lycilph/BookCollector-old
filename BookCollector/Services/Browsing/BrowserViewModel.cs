using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Framework.Core.Dialogs;
using ReactiveUI;

namespace BookCollector.Services.Browsing
{
    public class BrowserViewModel : ReactiveScreen, IHaveDoneTask
    {
        public TaskCompletionSource<bool> TaskCompletionSource { get; set; }
        public Task Done { get { return TaskCompletionSource.Task; } }

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set { this.RaiseAndSetIfChanged(ref _Url, value); }
        }
    }
}
