using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Services.Repository
{
    public class ImageDownloader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly BookRepository book_repository;
        private readonly BlockingCollection<ImportedBook> queue = new BlockingCollection<ImportedBook>();
        private CancellationTokenSource cts;
        private Task task;

        public ImageDownloader(BookRepository book_repository)
        {
            this.book_repository = book_repository;
        }

        private static string GetImageFolder()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(folder, "Images");
        }

        private static void EnsureImageFolderExists()
        {
            Directory.CreateDirectory(GetImageFolder());
        }

        private static string GetImageFilename(ImportedBook imported_book)
        {
            var extension = Path.GetExtension(imported_book.ImageLinks.ImageLink);
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg";

            return Path.Combine(GetImageFolder(),  imported_book.Book.Id + extension);
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
            EnsureImageFolderExists();

            cts = new CancellationTokenSource();
            task = Task.Factory.StartNew(() =>
            {
                using (var client = new WebClient())
                {
                    foreach (var imported_book in queue.GetConsumingEnumerable(cts.Token))
                    {
                        logger.Trace("Processing {0}", imported_book.Book.Title);

                        try
                        {
                            if (!string.IsNullOrWhiteSpace(imported_book.ImageLinks.ImageLink))
                            {
                                var filename = GetImageFilename(imported_book);
                                client.DownloadFile(imported_book.ImageLinks.ImageLink, filename);
                                imported_book.Book.Image = filename;
                            }

                            logger.Trace("Image for {0} downloaded", imported_book.Book.Title);
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

            var path = GetFilename();
            var items = queue.Select(b => new { BookId = b.Book.Id, b.ImageLinks });
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private void Load()
        {
            logger.Trace("Loading download queue");

            var path = GetFilename();
            if (!File.Exists(path))
                return;

            var dummy_links = new ImageLinks();
            var dummy = new [] { new {BookId = string.Empty, Imagelinks = dummy_links} };

            var json = File.ReadAllText(path);
            var links = JsonConvert.DeserializeAnonymousType(json, dummy);

            links.Select(l => new ImportedBook
            {
                Book = book_repository.Get(l.BookId),
                ImageLinks = l.Imagelinks
            })
            .Apply(queue.Add);
        }

        public void Add(IEnumerable<ImportedBook> imported_books)
        {
            logger.Trace("Adding {0} books to the image download queue", imported_books.Count());
            imported_books.Apply(queue.Add);
        }
    }
}
