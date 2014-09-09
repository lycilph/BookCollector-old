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
using BookCollector.Data;
using HtmlAgilityPack;
using NLog;
using RestSharp;

namespace BookCollector.Goodreads
{
    [Export(typeof(GoodreadsApi))]
    public class GoodreadsApi
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ApplicationSettings application_settings;
        private readonly RestClient client = new RestClient("https://www.goodreads.com");
        private string key;
        private DateTime last_execution_time_stamp;
        private CancellationTokenSource cts;
        private Task worker_task;

        private readonly List<ConcurrentQueue<Book>> queues = new List<ConcurrentQueue<Book>>();  
        private readonly ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

        [ImportingConstructor]
        public GoodreadsApi(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;
        }

        public void Initialize()
        {
            logger.Trace("Initialize");

            last_execution_time_stamp = DateTime.Now.AddSeconds(-1);

            var base_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fullpath = Path.Combine(base_dir, "goodreads.apikey");

            if (!File.Exists(fullpath))
                throw new InvalidDataException("Couldn't not find the Goodreads.apikey file!");

            key = File.ReadAllText(fullpath);
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidDataException("Invalid Goodreads api key found");

            cts = new CancellationTokenSource();
            worker_task = Task.Factory.StartNew(() => ProcessQueues(cts.Token), cts.Token);
        }

        private void ProcessQueues(CancellationToken token)
        {
            while (true)
            {
                rwl.EnterReadLock();
                try
                {
                    if (queues.Count == 0 || queues.All(q => q.IsEmpty))
                    {
                        Thread.Sleep(500);
                        logger.Trace("Nothing to process, sleeping");
                    }
                    else
                    {
                        foreach (var queue in queues)
                        {
                            if (rwl.WaitingWriteCount > 0)
                                break;

                            Book book;
                            if (!queue.TryDequeue(out book)) continue;

                            if (book.Status != BookStatus.Processed)
                            {
                                logger.Trace("Processing [{0}]", book.Title);
                                UpdateInformation(book);
                            }
                            else
                                logger.Trace("Skipping [{0}], already processed", book.Title);
                        
                            if (token.IsCancellationRequested)
                                goto break_point;
                        }
                    }
                }
                finally
                {
                    rwl.ExitReadLock();
                }

                break_point:
                if (token.IsCancellationRequested)
                    break;
            }
        }

        public void Shutdown()
        {
            logger.Trace("Shutdown");

            cts.Cancel();
            worker_task.Wait();
        }

        private string Execute(IRestRequest request)
        {
            request.AddParameter("key", key);
            var response = client.Execute(request);
            last_execution_time_stamp = DateTime.Now;

            return response.Content;
        }

        private T Execute<T>(IRestRequest request) where T : new()
        {
            request.AddParameter("key", key);
            var response = client.Execute<T>(request);
            last_execution_time_stamp = DateTime.Now;

            return response.Data;
        }

        private Task Delay()
        {
            var now = DateTime.Now;
            var next_execution = last_execution_time_stamp.AddSeconds(1);
            var difference = next_execution.Subtract(now);
            var delay = (difference.Milliseconds > 0 ? difference.Milliseconds : 0);

            logger.Trace("Waiting for {0} ms", delay);

            return Task.Delay(delay);
        }

        public GoodreadsResponse GetBookInformationByIsbn(string isbn)
        {
            logger.Trace("GetBookInformationByIsbn");
            Delay().Wait();

            var request = new RestRequest("book/isbn");
            request.AddParameter("isbn", isbn);
            request.AddParameter("format", "xml");

            return Execute<GoodreadsResponse>(request);
        }

        public GoodreadsResponse GetBookInformationById(string id)
        {
            logger.Trace("GetBookInformationById");
            Delay().Wait();

            var request = new RestRequest("book/show/{id}");
            request.AddUrlSegment("id", id);
            request.AddParameter("format", "xml");

            return Execute<GoodreadsResponse>(request);
        }

        public GoodreadsResponse GetBookInformationByTitle(string title)
        {
            logger.Trace("GetBookInformationByTitle");
            Delay().Wait();

            var parameter = title.Replace(' ', '+');

            var request = new RestRequest("book/title");
            request.AddParameter("format", "xml");
            request.AddParameter("title", parameter);

            return Execute<GoodreadsResponse>(request);
        }

        private GoodreadsResponse GetInformation(Book book)
        {
            logger.Trace("GetInformation");

            if (!string.IsNullOrWhiteSpace(book.ISBN))
                return GetBookInformationByIsbn(book.ISBN);

            if (!string.IsNullOrWhiteSpace(book.Title))
                return GetBookInformationByTitle(book.Title);

            return null;
        }

        private string GetImage(GoodreadsBook book)
        {
            logger.Trace("GetImage");

            var extension = Path.GetExtension(book.ImageUrl);
            var filename = Path.GetInvalidFileNameChars().Aggregate(book.Title, (current, c) => current.Replace(c, '_'));
            filename = application_settings.GetFullPath(filename + extension);

            if (File.Exists(filename))
                return filename;

            var url = book.ImageUrl;
            if (url.ToLowerInvariant().Contains("nophoto"))
                url = ScrapePageForImage(book.Link);

            if (url == null)
                return null;

            try
            {
                using (var web_client = new WebClient())
                {
                    logger.Trace("Starting download of " + url);
                    web_client.DownloadFile(url, filename);
                    logger.Trace("Downloaded");
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            return filename;
        }

        private static string ScrapePageForImage(string link)
        {
            logger.Trace("ScrapePageForImage - " + link);

            var hw = new HtmlWeb();
            var doc = hw.Load(link);

            logger.Trace("Page loaded");

            var nodes = doc.DocumentNode.SelectNodes(@"//img[@id='coverImage']");
            return nodes != null && nodes.Any() ? nodes.First().Attributes["src"].Value : null;
        }


        public void UpdateInformation(Book book)
        {
            var response = GetInformation(book);
            if (response != null)
            {
                if (string.IsNullOrWhiteSpace(book.GoodreadsId))
                    book.GoodreadsId = response.Book.Id;
                if (string.IsNullOrWhiteSpace(book.ISBN))
                    book.ISBN = response.Book.ISBN;
                if (string.IsNullOrWhiteSpace(book.ISBN13))
                    book.ISBN13 = response.Book.ISBN13;
         
                book.Image = GetImage(response.Book);
            }

            book.Status = BookStatus.Processed;
        }

        public ConcurrentQueue<Book> RequestUpdateQueue()
        {
            logger.Trace("RequestUpdateQueue");

            rwl.EnterWriteLock();
            try
            {
                var queue = new ConcurrentQueue<Book>();
                queues.Add(queue);
                return queue;
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        public void ShutdownUpdateQueue(ConcurrentQueue<Book> queue)
        {
            logger.Trace("ShutdownUpdateQueue");

            rwl.EnterWriteLock();
            try
            {
                if (!queues.Remove(queue))
                    throw new InvalidDataException();
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }
    }
}
