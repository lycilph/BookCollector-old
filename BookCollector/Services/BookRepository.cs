using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using NLog;

namespace BookCollector.Services
{
    [Export(typeof(BookRepository))]
    public class BookRepository
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Load()
        {
            logger.Trace("Loading");
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(folder, "books.txt");

            if (File.Exists(path))
            {
            }
        }

        public void Save()
        {
            logger.Trace("Saving");
        }
    }
}
