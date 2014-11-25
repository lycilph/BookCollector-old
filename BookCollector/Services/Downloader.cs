using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Services.Import;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Services
{
    [Export(typeof(Downloader))]
    public class Downloader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly BlockingCollection<ImportedBook> queue = new BlockingCollection<ImportedBook>();
        private CancellationTokenSource cts;
        private Task task;

        public void Start()
        {
            logger.Trace("Starting");

            cts = new CancellationTokenSource();
            task = Task.Factory.StartNew(() =>
            {
                foreach (var book in queue.GetConsumingEnumerable(cts.Token))
                {
                    logger.Trace("Processing {0}", book.Book.Title);
                    //Thread.Sleep(250);
                }
            }, cts.Token);
        }

        public async void Stop()
        {
            try
            {
                cts.Cancel();
                await task;
            }
            catch (OperationCanceledException)
            {
                logger.Trace("Download stopped");
            }
        }

        public void AddRange(IEnumerable<ImportedBook> books)
        {
            books.Apply(queue.Add);
        }
    }
}
