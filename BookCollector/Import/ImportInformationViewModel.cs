using System.ComponentModel.Composition;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace BookCollector.Import
{
    [Export(typeof(ImportInformationViewModel))]
    public class ImportInformationViewModel : ReactiveScreen
    {
        private ReactiveList<string> _Messages = new ReactiveList<string>();
        public ReactiveList<string> Messages
        {
            get { return _Messages; }
            set { this.RaiseAndSetIfChanged(ref _Messages, value); }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Messages.Clear();
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }
    }
}
