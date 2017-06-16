using System;
using BookCollector.Framework.MVVM;

namespace BookCollector.Shell
{
    public class ShellFacade : IShellFacade
    {
        public enum CommandPosition { Left, Right };

        private IShellViewModel shell_view_model;
        private IShellView shell_view;

        public ShellFacade(IShellViewModel shell_view_model, IShellView shell_view)
        {
            this.shell_view_model = shell_view_model;
            this.shell_view = shell_view;
        }

        public void AddCommand(IWindowCommand command, CommandPosition position)
        {
            if (position == CommandPosition.Left)
                shell_view_model.LeftShellCommands.Add(command);
            else
                shell_view_model.RightShellCommands.Add(command);
        }

        public void AddFlyout(IFlyout flyout)
        {
            shell_view_model.ShellFlyouts.Add(flyout);
        }

        public void Show()
        {
            shell_view.Show();
        }

        public void ShowMainContent(IScreen content, bool is_fullscreen = false)
        {
            shell_view_model.ShowMainContent(content, is_fullscreen);
        }
    }
}
