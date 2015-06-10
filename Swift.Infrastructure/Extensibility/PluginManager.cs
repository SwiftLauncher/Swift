using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Plugins;
using Swift.Extensibility.Services.Logging;
using Swift.Extensibility.UI;
using Swift.Toolkit;

namespace Swift.Infrastructure.Extensibility
{
    [Export(typeof(IPluginManager))]
    public class PluginManager : IPluginManager
    {
        public IList<MenuItem> MenuItems { get; } = new ThreadSafeObservableCollection<MenuItem>();

        /// <summary>
        /// Gets the list of all loaded plugins.
        /// </summary>
        [ImportMany(typeof(IPlugin))]
        public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins { get; private set; }

        public List<string> UntrustedPlugins { get; }

        private ILoggingChannel _log;

        [ImportingConstructor]
        public PluginManager(ILogger logger)
        {
            UntrustedPlugins = new List<string>();
            _log = logger.GetChannel<PluginManager>();
        }
    }
}
