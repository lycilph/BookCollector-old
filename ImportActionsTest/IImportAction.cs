namespace ImportActionsTest
{
    public interface IImportAction
    {
        IImportActionViewModel ViewModel { get; }
        void Execute();
    }
}
