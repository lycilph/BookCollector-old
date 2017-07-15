namespace BookCollector.Framework.MVVM
{
    public class ScreenConfiguration
    {
        public bool IsFullscreen { get; set; }
        public bool IsFlyout { get; set; }
        public bool ShowCollectionCommand { get; set; }
        public string MainContent { get; set; }
        public string HeaderContent { get; set; }
        public string MenuContent { get; set; }

        public ScreenConfiguration(string main_content, bool is_flyout)
        {
            MainContent = main_content;
            IsFlyout = is_flyout;
        }
        public ScreenConfiguration(string main_content, bool is_fullscreen, bool show_collection_command) : this(main_content, string.Empty, string.Empty, is_fullscreen, show_collection_command) { }
        public ScreenConfiguration(string main_content, string header_content, string menu_content, bool is_fullscreen, bool show_collection_command)
        {
            MainContent = main_content;
            HeaderContent = header_content;
            MenuContent = menu_content;
            IsFullscreen = is_fullscreen;
            IsFlyout = false;
            ShowCollectionCommand = show_collection_command;
        }
    }
}
