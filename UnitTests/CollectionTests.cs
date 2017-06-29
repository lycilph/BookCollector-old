using System.Linq;
using BookCollector.Data;
using BookCollector.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void TestCreateStandardCollection()
        {
            // Arrange
            //var application_model = new ApplicationModel();
            //var standard_collection = application_model.CreateDefaultCollection();

            //// Act

            //// Assert
            //Assert.AreEqual(standard_collection.Description.Name, Constants.DefaultCollectionName);
            //Assert.AreEqual(standard_collection.Description.Text, Constants.DefaultCollectionDescription);

            //Assert.AreEqual(standard_collection.Shelves.Count, 1);
            //Assert.AreEqual(standard_collection.Shelves.First().Name, Constants.AllShelfName);
            //Assert.AreEqual(standard_collection.Shelves.First().Description, Constants.AllShelfDescription);
        }

        [TestMethod]
        public void TestAddBook()
        {
            // Arrange
            //var application_model = new ApplicationModel();
            //var standard_collection = application_model.CreateDefaultCollection();
            //var all_books_shelf = standard_collection.Shelves.First();
            //var book = new Book("My Book");

            //// Act
            //standard_collection.AddBookToShelf(book, all_books_shelf);

            //// Assert
            //Assert.AreEqual(standard_collection.Books.Count, 1);
            //Assert.AreEqual(all_books_shelf.Books.Count, 1);
            //Assert.AreEqual(book.Shelves.Count, 1);
        }

        [TestMethod]
        public void TestRemoveBook()
        {
            // Arrange
            //var application_model = new ApplicationModel();
            //var standard_collection = application_model.CreateDefaultCollection();
            //var all_books_shelf = standard_collection.Shelves.First();
            //var book = new Book("My Book");

            //standard_collection.Books.Add(book);
            //all_books_shelf.Books.Add(book);
            //book.Shelves.Add(all_books_shelf);

            //// Act
            //standard_collection.RemoveBookFromShelf(book, all_books_shelf);

            //// Assert
            //Assert.AreEqual(standard_collection.Books.Count, 1);
            //Assert.AreEqual(all_books_shelf.Books.Count, 0);
            //Assert.AreEqual(book.Shelves.Count, 0);
        }
    }
}
