using System;
using BookCollector.Screens.Shell;

namespace BookCollector.Services
{
    public interface INavigationService
    {
        void Register(Type screen_type, ShellScreenPosition position = ShellScreenPosition.MainContent, bool show_collection_command = true, bool is_fullscreen = false);

        void NavigateTo(Type screen_type);
    }
}