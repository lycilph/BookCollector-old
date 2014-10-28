using BookCollector.Import;
using BookCollector.Shell;
using Caliburn.Micro;
using Framework.Core;

namespace BookCollector.Main
{
    public class MainViewModel : ViewModelBase
    {
        public void Import()
        {
            var shell = IoC.Get<IShellContent>();
            shell.ActiveItem = new ImportViewModel();
        }
    }
}
