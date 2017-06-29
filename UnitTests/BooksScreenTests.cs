using System.Linq;
using BookCollector.Data;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.ViewModels.Screens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class BooksScreenTests
    {
        [TestMethod]
        public void TestBookAndShelfs()
        {
            // Arrange
            //var application_model = new ApplicationModel();
            //var collection = application_model.CreateDefaultCollection();
            //application_model.CurrentCollection = collection;

            //collection.Add(new Shelf("Shelf 1"));
            //collection.Add(new Shelf("Shelf 2"));
            //collection.Add(new Shelf("Shelf 3"));

            //collection.AddBookToShelf(new Book("Book 1"), collection.Shelves[0]);
            //collection.AddBookToShelf(new Book("Book 2"), collection.Shelves[0]);
            //collection.AddBookToShelf(new Book("Book 3"), collection.Shelves[1]);
            //collection.AddBookToShelf(new Book("Book 4"), collection.Shelves[2]);

            //var books_screen = new BooksScreenViewModel(application_model);
            //var book = collection.Books.First();

            //// Act
            //books_screen.BookAndShelves.Apply(vm => vm.Book = book);
            //books_screen.BookAndShelves[1].IsChecked = true;

            //// Assert
            //Assert.AreEqual(book.Shelves.Count, 2);
            //Assert.IsTrue(book.Shelves.Contains(collection.Shelves[0]));
            //Assert.IsTrue(book.Shelves.Contains(collection.Shelves[1]));
        }
    }
}
