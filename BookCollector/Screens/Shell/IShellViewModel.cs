using Core;

namespace BookCollector.Screens.Shell
{
    public interface IShellViewModel : IShell, IViewAware
    {
        IWindowCommand SettingsCommand { get; }
        IWindowCommand CollectionCommand { get; }
        bool IsFullscreen { get; set; }

        void Show(IScreen screen, ShellScreenPosition position);
    }
}