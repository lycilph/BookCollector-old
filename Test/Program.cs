using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Google.Apis.Services;
using Newtonsoft.Json;
using RestSharp;
using Test.Amazon;

namespace Test
{
    class Program
    {
        static void Main()
        {
            //var books = SearchAmazon().Concat(SearchGoodReads()).Concat(SearchGoogleBooks()).ToList();
            //var json = JsonConvert.SerializeObject(books, Formatting.Indented);
            //var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Books.txt");
            //File.WriteAllText(path, json);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Books.txt");
            var json = File.ReadAllText(path);
            var books = JsonConvert.DeserializeObject<List<Book>>(json);


            var stemmer = new PorterStemmer();
            var stopword_handler = new StopWordHandler();
            var documents = new List<Document>();
            var sw = Stopwatch.StartNew();
            foreach (var book in books)
            {

                var text = book.Title + " " + book.Author;
                var terms = text.Split(" @$/#.-:&*+=[]?!(){},''\">_<;%\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                .Select(t => t.ToLowerInvariant())
                                .ToList();
                var stemmed_terms = terms.Where(t => !stopword_handler.IsStopWord(t))
                                         .Select(stemmer.StemTerm)
                                         .ToList();
                var term_frequency = stemmed_terms.GroupBy(t => t)
                                                  .ToDictionary(g => g.Key, g => g.Count());
                var term_squared_sum = term_frequency.Values.Sum(v => v*v);
                var term_norm = Math.Sqrt(term_squared_sum);
                var normalized_term_frequency = new Dictionary<string, double>();
                foreach (var kvp in term_frequency)
                {
                    normalized_term_frequency[kvp.Key] = kvp.Value/term_norm;
                }

                documents.Add(new Document
                {
                    Book = book,
                    Terms = terms,
                    StemmedTerms = stemmed_terms,
                    TermFrequency = term_frequency,
                    NormalizedTermFrequency = normalized_term_frequency
                });
            }

            var query_terms = new List<string> { "gateway", "pohl" };
            var stemmed_query_terms = query_terms.Select(stemmer.StemTerm).ToList();

            var idf = new Dictionary<string, double>();
            foreach (var term in stemmed_query_terms)
            {
                idf[term] = Math.Log(documents.Count/(double) (1 + documents.Count(d => d.StemmedTerms.Contains(term))));
            }

            var query_tfidf = stemmed_query_terms.Select(t => idf[t]).ToList();
            var normalized_query_tfidf = query_tfidf.Normalize();

            foreach (var document in documents)
            {
                document.tfidf = new List<double>();
                foreach (var stemmed_query_term in stemmed_query_terms)
                {
                    double tf;
                    if (!document.NormalizedTermFrequency.TryGetValue(stemmed_query_term, out tf))
                        tf = 0.0;
                    document.tfidf.Add(tf*idf[stemmed_query_term]);
                }
                document.Normalizedtfidf = document.tfidf.Normalize();

                // Find cosine similarity
                var dot_product = document.Normalizedtfidf.Zip(normalized_query_tfidf, (v1, v2) => v1*v2).Sum();
                document.CosineSimilarity = dot_product;
            }

            documents.Sort((d1, d2) => d2.CosineSimilarity.CompareTo(d1.CosineSimilarity));
            foreach (var document in documents)
            {
                Console.WriteLine("[{0}] - {1:F}", document.Book.Title, document.CosineSimilarity);
            }
            sw.Stop();
            Console.WriteLine("Elapsed " + sw.ElapsedMilliseconds);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static T GetSettings<T>(string api_name) where T : class 
        {
            var settings_filename = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(n => n.Contains(api_name));
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(settings_filename))
            using (var sr = new StreamReader(s))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        // https://flyingpies.wordpress.com/2009/08/01/17/
        private static IEnumerable<Book> SearchAmazon()
        {
            var settings = GetSettings<AmazonSettings>("Amazon");
            var client = new AWSECommerceServicePortTypeClient("AWSECommerceServicePort");
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(settings.AccessKeyId, settings.SecretKey));

            var item_search = new ItemSearch
            {
                Request = new[]
                {
                    new ItemSearchRequest
                    {
                        SearchIndex = "Books",
                        Keywords = "gateway pohl",
                        ResponseGroup = new[] { "ItemAttributes", "Images" }
                    },
                },
                AWSAccessKeyId = settings.AccessKeyId,
                AssociateTag = settings.AssociateTag
            };
            var response = client.ItemSearch(item_search);

            return response.Items[0]
                           .Item
                           .Select(item => new Book
                           {
                               Title = item.ItemAttributes.Title,
                               Author = item.ItemAttributes.Author.First(),
                               Source = "Amazon"
                           });
        }


        // https://www.goodreads.com/api/index
        private static IEnumerable<Book> SearchGoodReads()
        {
            var settings = GetSettings<GoodreadsSettings>("Goodreads");
            var client = new RestClient("https://www.goodreads.com");
            var request = new RestRequest("search/index.xml");
            request.AddQueryParameter("q", "gateway pohl");
            request.AddQueryParameter("key", settings.ApiKey);
            var response = client.Execute<GoodreadsResponse>(request);

            return response.Data
                           .Results
                           .Select(work => new Book
                           {
                               Title = work.Title,
                               Author = work.Author.Name,
                               Source = "Goodreads"
                           });
        }

        // https://developers.google.com/books/docs/v1/using
        private static IEnumerable<Book> SearchGoogleBooks()
        {
            var settings = GetSettings<GoogleBooksSettings>("GoogleBooks");
            var books_service = new Google.Apis.Books.v1.BooksService(new BaseClientService.Initializer
            {
                ApplicationName = "BookCollector",
                ApiKey = settings.ApiKey
            });
            var request = books_service.Volumes.List("gateway pohl");
            var volumes = request.Execute();

            return volumes.Items
                          .Select(volume => new Book
                          {
                              Title = volume.VolumeInfo.Title,
                              Author = (volume.VolumeInfo.Authors != null && volume.VolumeInfo.Authors.Any() ? volume.VolumeInfo.Authors.First() : string.Empty),
                              Source = "Google Books"
                          });
        }
    }
}
