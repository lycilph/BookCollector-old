using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Services;
using BookCollector.Services.Settings;
using Caliburn.Micro;
using Framework.Core.Shell;
using ReactiveUI;
using NLog;
using LogManager = NLog.LogManager;
using IScreen = Caliburn.Micro.IScreen;

namespace BookCollector.Shell
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ConductorShell<IScreen>, IHandle<ShellMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Stack<IScreen> items = new Stack<IScreen>();
        private readonly ApplicationSettings settings;
        private readonly BookRepository book_repository;
        private readonly Downloader downloader;
        private readonly IScreen main_view_model;

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator event_aggregator, 
                              ApplicationSettings settings,
                              BookRepository book_repository, 
                              Downloader downloader,
                              [Import("Main")] IScreen main_view_model)
        {
            this.settings = settings;
            this.book_repository = book_repository;
            this.main_view_model = main_view_model;
            this.downloader = downloader;

            event_aggregator.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            logger.Trace("Shell initializing");

            base.OnInitialize();
            DisplayName = "Book Collector";

            settings.Load();
            book_repository.Load();
            downloader.Start();

            Show(main_view_model);
        }

        protected override void OnDeactivate(bool close)
        {
            logger.Trace(string.Format("Shell deactivating ({0})", close));

            base.OnDeactivate(close);

            if (close)
            {
                settings.Save();
                book_repository.Save();
                downloader.Stop();
            }
        }

        protected void Back()
        {
            items.Pop();
            ActivateItem(items.Peek());
        }

        protected void Show(IScreen view_model)
        {
            items.Push(view_model);
            ActivateItem(view_model);
        }

        public void Handle(ShellMessage message)
        {
            logger.Trace("Shellmessage (Kind = {0})", Enum.GetName(typeof(ShellMessage.MessageKind), message.Kind));

            switch (message.Kind)
            {
                case ShellMessage.MessageKind.Back:
                    Back();
                    break;
                case ShellMessage.MessageKind.Show:
                    Show(message.ViewModel);
                    break;
                case ShellMessage.MessageKind.Exit:
                    TryClose();
                    break;
                case ShellMessage.MessageKind.Text:
                    Text = message.Text;
                    break;
                case ShellMessage.MessageKind.Busy:
                    IsBusy = message.Busy;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}