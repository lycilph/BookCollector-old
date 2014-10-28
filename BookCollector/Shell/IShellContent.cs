using Framework.Core;

namespace BookCollector.Shell
{
    public interface IShellContent
    {
        IViewModel ActiveItem { get; set; }
    }
}