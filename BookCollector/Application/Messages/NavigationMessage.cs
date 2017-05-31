namespace BookCollector.Application.Messages
{
    public class NavigationMessage
    {
        public string ScreenName { get; set; }

        public NavigationMessage(string name)
        {
            ScreenName = name;
        }
    }
}
