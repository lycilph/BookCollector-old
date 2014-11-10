namespace BookCollector.Services
{
    public abstract class ApiBase : IApi
    {
        public string Name { get; protected set; }
        public abstract bool IsAuthenticated { get; }

        protected ApiBase(string name)
        {
            Name = name;
        }
    }
}
