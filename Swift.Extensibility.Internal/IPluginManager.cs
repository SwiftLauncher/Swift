using System.Collections.Generic;
using Swift.Extensibility.UI;

namespace Swift.Extensibility.Internal
{
    public interface IPluginManager
    {
        IList<MenuItem> MenuItems { get; }
    }
}
