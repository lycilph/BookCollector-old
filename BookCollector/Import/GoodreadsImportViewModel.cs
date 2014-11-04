using System;
using BookCollector.Services.Goodreads;
using Caliburn.Micro.ReactiveUI;
using NLog;
using ReactiveUI;

namespace BookCollector.Import
{
    public class GoodreadsImportViewModel : ReactiveScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IProgress<string> progress;
        private readonly GoodreadsApi api;

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { this.RaiseAndSetIfChanged(ref _Message, value); }
        }

        public GoodreadsImportViewModel(GoodreadsApi api)
        {
            progress = new Progress<string>(s => Message = s);
            this.api = api;
        }

        protected override async void OnViewLoaded(object view)
        {
            logger.Trace("Initializing");

            base.OnInitialize();

            var temp = await api.GetBooksAsync(progress, 50);
            //Books = temp.Select(b => new Book { Title = b.Title, Description = b.Description }).ToList();
        }
    }
}
