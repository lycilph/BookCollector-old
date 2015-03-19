using BookCollector.Controllers.Misc;
using Panda.ApplicationCore.Shell;

namespace BookCollector.Controllers
{
    public interface IStatusController
    {
        string MainStatusText { set; }
        string AuxiliaryStatusText { set; }
        bool IsBusy { get; set; }
        void ClearStatusText();
        BusyWithMessage BusyWithMessage(string message);
        void AddFlyout(IFlyout flyout);
        void RemoveFlyout(IFlyout flyout);
    }
}