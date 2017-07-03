using BookCollector.Framework.Logging;

namespace BookCollector.Models
{
    public class ApplicationModel : IApplicationModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private ICollectionModel collection_model;

        public ICollectionModel CollectionModel { get { return collection_model; } }

        public ApplicationModel(ICollectionModel collection_model)
        {
            this.collection_model = collection_model;
        }

        public void Load()
        {
            log.Info("Loading");
            collection_model.CurrentCollection = collection_model.CreateDefaultCollection();
        }

        public void Save()
        {
        }
    }
}
