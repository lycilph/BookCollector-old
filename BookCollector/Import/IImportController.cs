namespace BookCollector.Import
{
    public interface IImportController
    {
        string Name { get; }
        
        void Start();
    }
}
