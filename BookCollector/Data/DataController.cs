using System.Collections.Generic;

namespace BookCollector.Data
{
    public class DataController : IDataController
    {
        public List<Book> Books { get; private set; } = new List<Book>();
    }
}
