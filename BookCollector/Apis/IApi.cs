namespace BookCollector.Apis
{
    public interface IApi
    {
        string Name { get; }
        bool IsAuthenticated { get; }
    }
}