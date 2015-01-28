using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Services;
using BookCollector.Utilities;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Controllers
{
    [Export(typeof(ImageDownloader))]
    public class ImageDownloader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly char[] invalid_filename_chars = Path.GetInvalidFileNameChars();
        private const string filename = "queue.txt";

        private readonly ApplicationSettings application_settings;
        private BlockingCollection<DownloadQueueItem> queue = new BlockingCollection<DownloadQueueItem>();
        private CancellationTokenSource cts;
        private Task main_work_task;

        [ImportingConstructor]
        public ImageDownloader(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;
        }

        private string GetImageName(Book book)
        {
            if (!string.IsNullOrWhiteSpace(book.ISBN10))
                return book.ISBN10.ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(book.ISBN13))
                return book.ISBN13.ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(book.Asin))
                return book.Asin.ToUpperInvariant();
            if (!string.IsNullOrWhiteSpace(book.Title))
                return new string(book.Title.Where(ch => !invalid_filename_chars.Contains(ch)).ToArray());
            return book.Id.ToUpperInvariant();
        }

        private string GetImageFilename(string url, string name, string postfix)
        {
            var extension = Path.GetExtension(url);
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg";
            var image_filename = string.Format("{0}_{1}{2}", name, postfix, extension);
            return Path.Combine(application_settings.ImageDir, image_filename);
        }

        private string GetPath(CollectionDescription collection)
        {
            var dir = Path.Combine(application_settings.DataDir, collection.Id);
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, filename);
        }

        public void Start()
        {
            logger.Trace("Starting image download queue");

            cts = new CancellationTokenSource();
            main_work_task = Task.Factory.StartNew(() =>
            {
                using (var client = new WebClient())
                {
                    foreach (var imported_book in queue.GetConsumingEnumerable(cts.Token))
                    {
                        logger.Trace("Processing [{0}] ({1} left in queue)", imported_book.Book.Title, queue.Count);

                        try
                        {
                            foreach (var image_link in imported_book.ImageLinks)
                            {
                                if (string.IsNullOrWhiteSpace(image_link.Url))
                                    continue;

                                var image_name = GetImageName(imported_book.Book);
                                var image_filename = GetImageFilename(image_link.Url, image_name, image_link.Property);

                                // Only download image, if it does not already exists
                                if (File.Exists(image_filename))
                                    logger.Trace("Image {0} already exists", image_filename);
                                else
                                    client.DownloadFile(image_link.Url, image_filename);

                                var property = typeof (Book).GetProperty(image_link.Property);
                                property.SetValue(imported_book.Book, image_filename);
                            }

                            logger.Trace("Image(s) for [{0}] downloaded", imported_book.Book.Title);
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
                main_work_task.Wait();
            }
            catch (Exception)
            {
                logger.Trace("Image download queue stopped");
            }
        }

        public void Clear()
        {
            Stop();
            queue = new BlockingCollection<DownloadQueueItem>();
            Start();
        }

        public void Save(CollectionDescription collection)
        {
            var path = GetPath(collection);
            logger.Trace("Saving (path = {0})", path);
            JsonExtensions.SerializeToFile(path, queue);
        }

        public void Load(CollectionDescription collection, Func<string, Book> find)
        {
            // Always clear the queue
            queue = new BlockingCollection<DownloadQueueItem>();

            var path = GetPath(collection);
            if (!File.Exists(path))
            {
                logger.Trace("No queue found");
                return;
            }

            logger.Trace("Loading (path = {0})", path);
            var items = JsonExtensions.DeserializeFromFile<List<DownloadQueueItem>>(path);
            items.Apply(i =>
            {
                i.Book = find(i.BookId);
                queue.Add(i);
            });
        }

        public void Add(IEnumerable<DownloadQueueItem> items)
        {
            var items_list = items.ToList();
            logger.Trace("Adding {0} books to the image download queue", items_list.Count());
            items_list.Apply(queue.Add);
        }
    }
}
