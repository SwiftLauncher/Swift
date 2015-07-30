using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Plugins;
using Swift.Extensibility.Services;
using Swift.Extensibility.Services.Logging;
using Swift.Extensibility.Services.Profile;
using Swift.Extensibility.Services.Settings;
using Swift.Modules.Services;

namespace Swift.Infrastructure.Services
{
    [Export(typeof(IPluginServices))]
    public class PluginServices : IPluginServices, IInitializationAware, IShutdownAware
    {
        private readonly ILoggingChannel _log;

        [ImportingConstructor]
        public PluginServices(ILogger logger)
        {
            _log = logger.GetChannel<PluginServices>();
        }

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

        [Import]
        private IVaultManager _storageManager;
        
        private Dictionary<string, SettingsStore> _settingsStores;

        /// <summary>
        /// Gets the settings store.
        /// </summary>
        /// <typeparam name="TPlugin">The type of the plugin.</typeparam>
        /// <returns>The settings store.</returns>
        public ISettingsStore GetSettingsStore<TPlugin>() where TPlugin : IPlugin
        {
            if (_settingsStores.ContainsKey(typeof(TPlugin).FullName))
            {
                return _settingsStores[typeof(TPlugin).FullName];
            }
            var s = new SettingsStore();
            _settingsStores[typeof(TPlugin).FullName] = s;
            return s;
        }

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

        public int InitializationPriority => InternalConstants.InitializationPriorities.PluginServices;


        public int ShutdownPriority => InternalConstants.ShutdownPriorities.PluginServices;

        public UserProfile GetCurrentUser()
        {
            // TODO
            return new UserProfile("bla", Encoding.UTF8.GetBytes("bla"));
        }
    }
}
