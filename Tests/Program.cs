using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var b1 = new Book("Book 1");
            var b2 = new Book("Book 2");
            var b3 = new Book("Book 3");

            var shelf1 = new Shelf("Shelf 1");
            var shelf2 = new Shelf("Shelf 2");
            var shelf3 = new Shelf("Shelf 3");

            b1.Shelves = new List<Shelf> { shelf1 };
            b2.Shelves = new List<Shelf> { shelf1, shelf2 };
            b3.Shelves = new List<Shelf> { shelf1, shelf2, shelf3 };

            var books = new List<Book> { b1, b2, b3 };
            // Flatten books into (book, shelf) pairs
            var flattened = books.SelectMany(b => b.Shelves
                                                   .Select(s => new { book = b, shelf = s }));
            // Group into (shelf, list of books)
            var grouped = flattened.GroupBy(f => f.shelf, f => f.book).ToList();

            foreach (var b in books)
                b.Shelves.Clear();

            foreach (var g in grouped)
            {
                Console.WriteLine($"Shelf {g.Key.Name}");
                foreach (var b in g)
                    Console.WriteLine($"Book {b.Title}");
            }
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
