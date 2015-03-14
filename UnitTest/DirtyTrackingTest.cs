using System.Linq;
using BookCollector.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class DirtyTrackingTest
    {
        [TestMethod]
        public void TestCreation()
        {
            var user = new User();
            Assert.AreEqual(false, user.IsDirty);
        }

        [TestMethod]
        public void TestPropertyChange()
        {
            var user = new User {Name = "Test"};
            Assert.AreEqual(true, user.IsDirty);
        }

        [TestMethod]
        public void TestDirtyReset()
        {
            var user = new User { Name = "Test" };
            user.IsDirty = false;
            Assert.AreEqual(false, user.IsDirty);
        }

        [TestMethod]
        public void TestCollectionPropertyChange()
        {
            var user = new User();
            user.Collections.Add(new Collection("Test"));
            Assert.AreEqual(true, user.IsDirty);
        }

        [TestMethod]
        public void TestSubPropertyChange()
        {
            var user = new User();
            user.Collections.Add(new Collection("Test"));
            user.IsDirty = false;
            user.Collections.First().Name = "New name";
            Assert.AreEqual(true, user.IsDirty);
        }
    }
}
