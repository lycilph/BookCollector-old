using Framework.Docking;

namespace BookCollector.Start
{
    public interface IStartTool : ITool
    {
        void NewCollection();
        void OpenCollection();
    }
}
