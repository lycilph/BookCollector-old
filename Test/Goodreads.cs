using System.Collections.Generic;

namespace Test
{
    public class GoodreadsResponse
    {
        public List<GoodreadsWork> Results { get; set; }
    }

    public class GoodreadsWork
    {
        public string Title { get; set; }
        public GoodreadsAuthor Author { get; set; }
    }

    public class GoodreadsAuthor
    {
        public string Name { get; set; }
    }
}
