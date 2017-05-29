namespace BookCollector.Framework.Messages
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
