namespace ImportActionsTest
{
    public class ImportActionBase : IImportAction
    {
        public IImportActionViewModel ViewModel { get; protected set; }

        public virtual void Execute()
        {
            
        }
    }
}
