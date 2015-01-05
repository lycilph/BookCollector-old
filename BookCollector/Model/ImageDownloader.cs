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
using Caliburn.Micro;
using Newtonsoft.Json;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Model
{
    [Export(typeof(ImageDownloader))]
    public class ImageDownloader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly BookRepository book_repository;
        private readonly BlockingCollection<ImportedBook> queue = new BlockingCollection<ImportedBook>();
        private CancellationTokenSource cts;
        private Task task;

        [ImportingConstructor]
        public ImageDownloader(BookRepository book_repository)
        {
            this.book_repository = book_repository;
        }

        private static string GetImageFolder()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentNullException();
            return Path.Combine(folder, "Images");
        }

        private static void EnsureImageFolderExists()
        {
            Directory.CreateDirectory(GetImageFolder());
        }

        private static string GetImageFilename(string url, string name, string postfix)
        {
            var extension = Path.GetExtension(url);
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg";
            var filename = string.Format("{0}_{1}{2}", name, postfix, extension);
            return Path.Combine(GetImageFolder(), filename);
        }

        private static string GetFilename()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentNullException();
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
                            foreach (var image_link in imported_book.ImageLinks)
                            {
                               if (!string.IsNullOrWhiteSpace(image_link.Url))
                               {
                                   var filename = GetImageFilename(image_link.Url, imported_book.Book.Id, image_link.Property);
                                   client.DownloadFile(image_link.Url, filename);

                                   var property = typeof (Book).GetProperty(image_link.Property);
                                   property.SetValue(imported_book.Book, filename);

                                   Thread.Sleep(100);
                               }
                            }

                            logger.Trace("Image(s) for {0} downloaded", imported_book.Book.Title);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                        }

                        //Thread.Sleep(100);
                        for (var i = 0; i < 100 && !cts.Token.IsCancellationRequested; i++)
                            Thread.Sleep(50);
                    }
                }
            }, cts.Token);
        }

        public void Stop()
        {
            try
            {
                logger.Trace("Stopping image download queue");
                cts.Cancel();
                task.Wait();
            }
            catch (AggregateException)
            {
                logger.Trace("Image download queue stopped");
            }

            // Save queue
            Save();
        }

        private void Save()
        {
            logger.Trace("Saving image download queue");

            var path = GetFilename();
            var items = queue.Select(b => new { BookId = b.Book.Id, b.ImageLinks });
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private void Load()
        {
            logger.Trace("Loading image download queue");

            var path = GetFilename();
            if (!File.Exists(path))
                return;

            var dummy_links = new [] { new ImageLink() };
            var dummy = new [] { new {BookId = string.Empty, Imagelinks = dummy_links} };

            var json = File.ReadAllText(path);
            var links = JsonConvert.DeserializeAnonymousType(json, dummy);

            links.Select(l => new ImportedBook
            {
                Book = book_repository.Get(l.BookId),
                ImageLinks = l.Imagelinks.ToList()
            })
            .Apply(queue.Add);
        }

        public void Add(IEnumerable<ImportedBook> imported_books)
        {
            var imported_books_list = imported_books.ToList();
            logger.Trace("Adding {0} books to the image download queue", imported_books_list.Count());
            imported_books_list.Apply(queue.Add);
        }
    }
}
