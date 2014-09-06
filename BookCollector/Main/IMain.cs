using BookCollector.Data;
using Framework.Docking;

namespace BookCollector.Main
{
    public interface IMain : IContent
    {
        Info Info { get; set; }
    }
}
