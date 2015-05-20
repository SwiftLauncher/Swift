using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility;
using Swift.Extensibility.Plugins;
using Swift.Extensibility.Services;
using Swift.Extensibility.UI;
using Swift.Toolkit;
using System.Linq;
using Swift.Extensibility.Services.Logging;

namespace Swift.Infrastructure.Extensibility
{
    public class PluginManager : IPluginManager, IInitializationAware, IShutdownAware
    {
        #region Properties

        private IList<MenuItem> _menuItems = new ThreadSafeObservableCollection<MenuItem>();
        public IList<MenuItem> MenuItems
        {
            get { return _menuItems; }
        }

        [ImportMany(typeof(IPlugin))]
        private IEnumerable<Lazy<IPlugin, IPluginMetadata>> _plugins = null;
        /// <summary>
        /// Gets the list of all loaded plugins.
        /// </summary>
        public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins
        {
            get
            {
                return _plugins;
            }
        }

        public List<string> UntrustedPlugins { get; private set; }

        #endregion

        private ILoggingChannel _log;

        private PluginManager()
        {
            UntrustedPlugins = new List<string>();
            _log = ServiceLocator.Current.GetInstance<ILogger>().GetChannel<PluginManager>();
        }

        /// <summary>
        /// Distributes the plugin services to all plugin service users.
        /// </summary>
        public void DistributePluginServices()
        {
            var users = ServiceLocator.Current.GetAllInstances<IPluginServiceUser>();
            var ps = ServiceLocator.Current.GetInstance<IPluginServices>();
            foreach (var user in users)
            {
                try
                {
                    user.SetPluginServices(ps); ;
                    _log.Log("Set PluginServices on '" + user.GetType().FullName);
                }
                catch (Exception ex)
                {
                    _log.Log("'" + ex.GetType().Name + "' occurred while setting PluginServices on '" + user.GetType().FullName + "': " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Distributes the logging channels.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void DistributeLoggingChannels()
        {
            var ilss = ServiceLocator.Current.GetAllInstances<ILogSource>();
            var logger = ServiceLocator.Current.GetInstance<ILogger>();
            foreach (var ils in ilss)
            {
                try
                {
                    ils.SetLoggingChannel(logger.GetChannel(ils.GetType().FullName));
                    _log.Log("LoggingChannel set on '" + ils.GetType().FullName + "'");
                }
                catch (Exception ex)
                {
                    _log.Log("'" + ex.GetType().Name + "' occurred while setting logging channel on '" + ils.GetType().FullName + "': " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Shuts down the plugins.
        /// </summary>
        public void ShutdownPlugins()
        {
            var isas = ServiceLocator.Current.GetAllInstances<IShutdownAware>();
            var l = new List<IShutdownAware>();
            foreach (var isa in isas)
            {
                try
                {
                    var p = isa.ShutdownPriority;
                    l.Add(isa);
                }
                catch (Exception ex)
                {
                    _log.Log("'" + ex.GetType().Name + "' occurred while trying to access ShutdownPriority of '" + isa.GetType().FullName + "'. It will not be shut down. (" + ex.Message + ")");
                }
            }
            foreach (var isa in l.OrderBy(_ => _.ShutdownPriority))
            {
                try
                {
                    //await Task.Run(() => isa.OnShutdown(new ShutdownEventArgs())).ConfigureAwait(false);
                    isa.OnShutdown(new ShutdownEventArgs());
                    _log.Log("'" + isa.GetType().FullName + "' shut down.");
                }
                catch (Exception ex)
                {
                    _log.Log("'" + ex.GetType().Name + "' occurred while shutting down '" + isa.GetType().FullName + "': " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Gets the initialization priority. Higher values lead to later initialization.
        /// </summary>
        /// <value>
        /// The initialization priority.
        /// </value>
        public int InitializationPriority
        {
            get { return InternalConstants.InitializationPriorities.PluginManager; }
        }

        /// <summary>
        /// Handles the <see cref="E:Initialization" /> event.
        /// </summary>
        /// <param name="args">The <see cref="InitializationEventArgs" /> instance containing the event data.</param>
        public void OnInitialization(InitializationEventArgs args)
        {
            DistributeLoggingChannels();
            DistributePluginServices();
            //InitializePlugins();
        }

        /// <summary>
        /// Gets the shutdown priority. Higher values lead to later shutdown.
        /// </summary>
        /// <value>
        /// The shutdown priority.
        /// </value>
        public int ShutdownPriority
        {
            get { return InternalConstants.ShutdownPriorities.PluginManager; }
        }

        /// <summary>
        /// Handles the <see cref="E:Shutdown" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ShutdownEventArgs" /> instance containing the event data.</param>
        public void OnShutdown(ShutdownEventArgs args)
        {
            //ShutdownPlugins();
        }

        public void InitializePlugins()
        {
            throw new NotSupportedException();
        }
    }
}
