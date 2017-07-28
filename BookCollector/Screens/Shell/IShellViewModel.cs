using Core;

namespace BookCollector.Screens.Shell
{
    public interface IShellViewModel : IShell, IViewAware
    {
        IWindowCommand SettingsCommand { get; }
        IWindowCommand CollectionCommand { get; }

        void Show(IScreen screen, ShellScreenPosition position, bool is_fullscreen = false);
    }
}