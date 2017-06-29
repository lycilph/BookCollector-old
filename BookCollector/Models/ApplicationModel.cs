namespace BookCollector.Models
{
    public class ApplicationModel : IApplicationModel
    {
        private ICollectionModel collection_model;

        public ApplicationModel(ICollectionModel collection_model)
        {
            this.collection_model = collection_model;
        }

        public void Load()
        {
            collection_model.CurrentCollection = collection_model.CreateDefaultCollection();
        }

        public void Save()
        {
        }
    }
}
