namespace BookCollector.Controllers
{
    public interface IStatusController
    {
        string MainStatusText { set; }
        string AuxiliaryStatusText { set; }
        bool IsBusy { get; set; }
    }
}