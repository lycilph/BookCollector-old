using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Data;
using BookCollector.Utils;
using HtmlAgilityPack;
using NLog;
using RestSharp;
using Caliburn.Micro;
using LogManager = NLog.LogManager;

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
        private TaskScheduler context;
        private readonly ConcurrentDictionary<Info, ConcurrentQueue<Book>> queue_dictionary = new ConcurrentDictionary<Info, ConcurrentQueue<Book>>();
        public ConcurrentQueue<Book> PriorityQueue { get; set; }
            
        [ImportingConstructor]
        public GoodreadsApi(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;
            PriorityQueue = new ConcurrentQueue<Book>();
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

            context = TaskScheduler.FromCurrentSynchronizationContext();
            cts = new CancellationTokenSource();
            worker_task = Task.Factory.StartNew(() => ProcessQueues(cts.Token), cts.Token);
        }

        private void ProcessQueues(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    return;

                ProcessPriorityQueue(token);

                if (token.IsCancellationRequested)
                    return;

                if (queue_dictionary.Count == 0 || queue_dictionary.Values.All(q => q.IsEmpty))
                {
                    logger.Trace("Nothing to process, sleeping");
                    Thread.Sleep(500);
                    continue;
                }

                foreach (var kvp in queue_dictionary)
                {
                    logger.Info("Processing queue for [{0}] {1} left", kvp.Key.DisplayName, kvp.Value.Count);

                    Book book;
                    if (kvp.Value.TryDequeue(out book))
                        ProcessBook(book);

                    if (token.IsCancellationRequested)
                        return;

                    ProcessPriorityQueue(token);

                    if (token.IsCancellationRequested)
                        return;
                }
            }
        }

        private void ProcessPriorityQueue(CancellationToken token)
        {
            Book book;
            if (!PriorityQueue.TryDequeue(out book)) return;

            logger.Info("Processing priority queue [{0}]", book.Title);

            var response = (book.Status != BookStatus.Processed ? UpdateInformation(book) : GetInformation(book));
            if (response.Book.SimilarBooks.Count == 0)
            {
                logger.Info("No similar books found");
                return;
            }

            if (token.IsCancellationRequested)
                return;

            response.Book.SimilarBooks.Select(gb => Mapper.MapPublicProperties(gb, new Book()))
                                      .Apply(b => PriorityQueue.Enqueue(b));

            Book similar_book;
            while (PriorityQueue.TryDequeue(out similar_book))
            {
                UpdateInformation(similar_book);

                if (token.IsCancellationRequested)
                    return;

                var temp = similar_book;
                Task.Factory.StartNew(() => book.SimilarBooks.Add(temp), CancellationToken.None, TaskCreationOptions.None, context);
            }
        }

        private void ProcessBook(Book book)
        {
            if (book.Status != BookStatus.Processed)
            {
                logger.Trace("Processing [{0}]", book.Title);
                UpdateInformation(book);
            }
            else
                logger.Trace("Skipping [{0}], already processed", book.Title);
        }

        public async void Shutdown()
        {
            logger.Trace("Cancelling and waiting for shutdown");

            cts.Cancel();
            await worker_task;

            logger.Trace("Shutdown");
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
            {
                logger.Trace("Image already found");
                return filename;
            }

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


        public GoodreadsResponse UpdateInformation(Book book)
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
                if (string.IsNullOrWhiteSpace(book.Description))
                    book.Description = response.Book.Description;
         
                book.Image = GetImage(response.Book);
            }

            book.Status = BookStatus.Processed;
            return response;
        }

        public ConcurrentQueue<Book> RequestQueue(Info info)
        {
            logger.Trace("RequestQueue");

            var queue = new ConcurrentQueue<Book>();
            if (!queue_dictionary.TryAdd(info, queue))
                throw new Exception("Couldn't add queue");

            return queue;
        }

        public void ShutdownQueue(Info info)
        {
            logger.Trace("ShutdownQueue");

            ConcurrentQueue<Book> queue;
            if (!queue_dictionary.TryRemove(info, out queue))
                throw new Exception("Couldn't remove queue");
        }

        public void ClearPriorityQueue()
        {
            
        }
    }
}
