using System.Collections.Generic;
using System.ComponentModel.Composition;
using Swift.Extensibility.UI;

namespace Swift.Extensibility
{
    [InheritedExport]
    public interface IPluginManager
    {
        IList<MenuItem> MenuItems { get; }

        /// <summary>
        /// Initializes the plugins.
        /// </summary>
        void InitializePlugins();

        /// <summary>
        /// Distributes the logging channels.
        /// </summary>
        void DistributeLoggingChannels();

        /// <summary>
        /// Distributes the plugin services to all plugin service users.
        /// </summary>
        void DistributePluginServices();

        /// <summary>
        /// Shuts down the plugins.
        /// </summary>
        void ShutdownPlugins();
    }
}
