using System;

namespace BookCollector.Controllers.Misc
{
    public class BusyWithMessage : IDisposable
    {
        private readonly IStatusController status_controller;

        public BusyWithMessage(IStatusController status_controller, string message)
        {
            this.status_controller = status_controller;

            status_controller.MainStatusText = message;
            status_controller.IsBusy = true;
        }

        public void Dispose()
        {
            status_controller.IsBusy = false;
            status_controller.MainStatusText = string.Empty;
        }
    }
}
