using System;
using System.Collections.Generic;

namespace Searching
{
    class Program
    {
        static void Main(string[] args)
        {
            var books = JsonExtensions.ReadFromFile<List<Book>>(@"C:\Private\Projects\BookCollector\books.json");
            var engine = new SearchEngine();
            engine.Index(books);

            var search_result = engine.Search("Space");
            for (int i = 0; i < search_result.Count; i++)
            {
                var result = search_result[i];
                Console.WriteLine($"{i+1}: Score {result.Score} - {result.Document.Book.Title}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
