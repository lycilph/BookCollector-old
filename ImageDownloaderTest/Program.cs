using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageDownloaderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Book> books;

            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(folder, "test.txt");

            if (File.Exists(filename))
            {
                Console.WriteLine("Reading file");
                var json = File.ReadAllText(filename);
                books = JsonConvert.DeserializeObject<List<Book>>(json);                
            }
            else
            {
                Console.WriteLine("Creating data");
                //books = Angie.FastList<Book>(100);                
                books = new List<Book>();
            }

            var bc = new BlockingCollection<Book>();
            foreach (var book in books)
            {
                bc.Add(book);
            }
            bc.CompleteAdding();

            var cts = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() =>
            {
                foreach (var book in bc.GetConsumingEnumerable(cts.Token))
                {
                    Console.WriteLine("Processed {0} - {1} left", book.Link, bc.Count);
                    Thread.Sleep(250);
                }
                Console.WriteLine("No more data left");
            }, cts.Token);

            Console.WriteLine("Press q to stop");
            Console.ReadKey();

            if (bc.Count > 0)
            {
                Console.WriteLine("Saving data");
                cts.Cancel();
                var json = JsonConvert.SerializeObject(bc.ToList(), Formatting.Indented);
                File.WriteAllText(filename, json);    
            }
            else
            {
                Console.WriteLine("Deleting file");
                File.Delete(filename);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
