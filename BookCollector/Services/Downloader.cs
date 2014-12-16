using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Services.Import;
using Caliburn.Micro;
using Newtonsoft.Json;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Services
{
    [Export(typeof(Downloader))]
    public class Downloader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly BookRepository book_repository;
        private readonly BlockingCollection<ImportedBook> queue = new BlockingCollection<ImportedBook>();
        private CancellationTokenSource cts;
        private Task task;

        [ImportingConstructor]
        public Downloader(BookRepository book_repository)
        {
            this.book_repository = book_repository;
        }

        private static string GetImageFilename(string name)
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(folder, "Images", name + ".jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }

        private static string GetFilename()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(folder, "download.txt");
        }

        public void Start()
        {
            logger.Trace("Starting");

            Load();

            cts = new CancellationTokenSource();
            task = Task.Factory.StartNew(() =>
            {
                using (var client = new WebClient())
                {
                    foreach (var book in queue.GetConsumingEnumerable(cts.Token))
                    {
                        logger.Trace("Processing {0}", book.Book.Title);

                        try
                        {
                            var filename = GetImageFilename(book.Book.Id.ToString());
                            client.DownloadFile(book.ImageLinks.ImageLink, filename);
                            book.Book.Image = filename;

                            logger.Trace("Image for {0} downloaded", book.Book.Title);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                        }

                        Thread.Sleep(100);
                        //for (var i = 0; i < 100 && !cts.Token.IsCancellationRequested; i++)
                        //    Thread.Sleep(50);
                    }
                }
            }, cts.Token);
        }

        public void Stop()
        {
            try
            {
                logger.Trace("Stopping download");
                cts.Cancel();
                task.Wait();
            }
            catch (AggregateException)
            {
                logger.Trace("Download stopped");
            }

            // Save queue
            Save();
        }

        private void Save()
        {
            logger.Trace("Saving download queue");

            queue.Apply(b => b.ImageLinks.BookId = b.Book.Id);

            var path = GetFilename();
            var links = queue.Select(b => b.ImageLinks);
            var json = JsonConvert.SerializeObject(links, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private void Load()
        {
            logger.Trace("Loading download queue");

            var path = GetFilename();
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            var links = JsonConvert.DeserializeObject<List<ImageLinks>>(json);
            var books = links.Select(l => new ImportedBook
            {
                Book = book_repository.Get(l.BookId),
                ImageLinks = l
            });
            AddRange(books);
        }

        public void AddRange(IEnumerable<ImportedBook> books)
        {
            logger.Trace("Adding {0} books to download queue", books.Count());
            books.Apply(queue.Add);
        }
    }
}
