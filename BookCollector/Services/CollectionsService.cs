using BookCollector.Data;

namespace BookCollector.Services
{
    public class CollectionsService : ICollectionsService
    {
        public Collection Current { get; set; }

        public void Initialize()
        {
            Current = new Collection()
            {
                DefaultShelf = new Shelf()
            };
        }
    }
}
