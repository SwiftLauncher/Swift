using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Swift.Infrastructure.Extensibility;
using Swift.Extensibility;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Services;
using Swift.Extensibility.Services.Logging;
using Swift.Extensibility.Services.Settings;
using Swift.Extensibility.UI;
using Swift.Modules.Services;

namespace Swift.PluginHandling
{
    [Export(typeof(IPluginServices))]
    public class PluginServices : IPluginServices, IInitializationAware, IShutdownAware
    {
        private ILoggingChannel _log;

        public PluginServices()
        {
            _log = ServiceLocator.Current.GetInstance<ILogger>().GetChannel<PluginServices>();
        }

        #region Service Query

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public TService GetService<TService>()
        {
            //if (typeof(TService) == typeof(ILogger))
            //{
            //    return (TService)ServiceLocator.Current.GetInstance<ILogger>();
            //}
            //throw new ArgumentException("The service type" + typeof(TService).FullName + "' cannot be retrieved because it is not known.");
            //TODO add security
            return ServiceLocator.Current.GetInstance<TService>();
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<TService> GetServices<TService>()
        {
            //if (typeof(TService) == typeof(ILogger))
            //{
            //    return ServiceLocator.Current.GetAllInstances<ILogger>().Cast<TService>();
            //}
            //throw new ArgumentException("The services of  type" + typeof(TService).FullName + "' cannot be retrieved because the service type is not known.");
            // TODO add security
            return ServiceLocator.Current.GetAllInstances<TService>();
        }

        #endregion

        [Import]
        private IVaultManager _storageManager = null;

        #region Settings

        private Dictionary<string, SettingsStore> _settingsStores;

        /// <summary>
        /// Gets the settings store.
        /// </summary>
        /// <typeparam name="TPlugin">The type of the plugin.</typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ISettingsStore GetSettingsStore<TPlugin>()
        {
            if (_settingsStores.ContainsKey(typeof(TPlugin).FullName))
            {
                return _settingsStores[typeof(TPlugin).FullName];
            }
            else
            {
                var s = new SettingsStore();
                _settingsStores[typeof(TPlugin).FullName] = s;
                return s;
            }
        }

        #endregion

        public void OnInitialization(InitializationEventArgs args)
        {
            try
            {
                _settingsStores = _storageManager.Vault.Retrieve<Dictionary<string, SettingsStore>>("PluginServices.SettingsStores");
            }
            catch (Exception ex)
            {
                _settingsStores = new Dictionary<string, SettingsStore>();
                _log.Log(ex.GetType().Name + " occurred while trying to retrieve SettingsStores (" + ex.Message + "). SettingsStores reset.");
            }
        }

        public void OnShutdown(ShutdownEventArgs args)
        {
            foreach (var s in _settingsStores.Values)
            {
                s.ClearListeners();
            }
            _storageManager.Vault.Store("PluginServices.SettingsStores", _settingsStores);
        }

        public int InitializationPriority
        {
            get { return InternalConstants.InitializationPriorities.PluginServices; } // initialize first
        }


        public int ShutdownPriority
        {
            get { return InternalConstants.ShutdownPriorities.PluginServices; } // shutdown last
        }

        public IUserProfile GetCurrentUser()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
