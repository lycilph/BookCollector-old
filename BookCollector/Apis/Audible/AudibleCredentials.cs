namespace BookCollector.Apis.Audible
{
    public class AudibleCredentials
    {
        public string CustomerId { get; set; }

        public AudibleCredentials()
        {
            CustomerId = string.Empty;
        }
    }
}
