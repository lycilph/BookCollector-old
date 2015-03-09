using System.Collections.Generic;

namespace BookCollector.Data
{
    public class User
    {
        public string Name { get; set; }
        public List<Collection> Collections { get; set; }

        public User()
        {
            Collections = new List<Collection>();
        }
    }
}
