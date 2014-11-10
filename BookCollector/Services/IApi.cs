namespace BookCollector.Services
{
    public interface IApi
    {
        string Name { get; }
        bool IsAuthenticated { get; }
    }
}