using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Services.Settings;
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

        private readonly ApplicationSettings application_settings;
        private BlockingCollection<ImportedBook> queue = new BlockingCollection<ImportedBook>();
        private CancellationTokenSource cts;
        private Task task;

        [ImportingConstructor]
        public ImageDownloader(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;
        }

        private string GetImageFilename(string url, string name, string postfix)
        {
            var extension = Path.GetExtension(url);
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg";
            var filename = string.Format("{0}_{1}{2}", name, postfix, extension);
            return Path.Combine(application_settings.ImageDir, filename);
        }

        public void Start()
        {
            logger.Trace("Starting image download queue");

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
                                if (string.IsNullOrWhiteSpace(image_link.Url))
                                    continue;

                                var filename = GetImageFilename(image_link.Url, imported_book.Book.Id, image_link.Property);
                                client.DownloadFile(image_link.Url, filename);

                                var property = typeof (Book).GetProperty(image_link.Property);
                                property.SetValue(imported_book.Book, filename);
                            }

                            logger.Trace("Image(s) for {0} downloaded", imported_book.Book.Title);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                        }

                        Thread.Sleep(100); // Be nice, when downloading images from websites :-)
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
        }

        public void Save(CollectionDescription collection)
        {
            logger.Trace("Saving image download queue " + collection);

            var path = Path.Combine(application_settings.DataDir, collection.Id + "_queue.txt");
            var items = queue.Select(b => new { BookId = b.Book.Id, b.ImageLinks });
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public void Load(CollectionDescription collection, BookRepository repository)
        {
            // Always clear the queue
            queue = new BlockingCollection<ImportedBook>();

            var path = Path.Combine(application_settings.DataDir, collection.Id + "_queue.txt");
            if (!File.Exists(path))
            {
                logger.Trace("No queue found for " + collection);
                return;
            }

            logger.Trace("Loading image download queue " + collection);
            var dummy_links = new[] { new ImageLink() };
            var dummy = new[] { new { BookId = string.Empty, Imagelinks = dummy_links } };

            var json = File.ReadAllText(path);
            var links = JsonConvert.DeserializeAnonymousType(json, dummy);

            links.Select(l => new ImportedBook
            {
                Book = repository.Get(l.BookId),
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
