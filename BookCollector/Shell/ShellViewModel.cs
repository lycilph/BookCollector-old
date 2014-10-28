using System.ComponentModel.Composition;
using BookCollector.Main;
using BookCollector.Services;
using Caliburn.Micro;
using Framework.Core;
using NLog;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace BookCollector.Shell
{
    [Export(typeof(IShell))]
    [Export(typeof(IShellContent))]
    public class ShellViewModel : ShellBase, IShellContent
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IViewModel _ActiveItem;
        public IViewModel ActiveItem
        {
            get { return _ActiveItem; }
            set { this.RaiseAndSetIfChanged(ref _ActiveItem, value); }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = "Book Collector";

            ActiveItem = new MainViewModel();

            var settings = IoC.Get<ApplicationSettings>();
            settings.Load();

            var book_repository = IoC.Get<BookRepository>();
            book_repository.Load();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
            {
                var settings = IoC.Get<ApplicationSettings>();
                settings.Save();

                var book_repository = IoC.Get<BookRepository>();
                book_repository.Save();
            }
        }
    }
}