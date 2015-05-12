using Swift.Extensibility.Services.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Swift.Extensibility.Internal;
using Swift.Toolkit.Data;
using Swift.Extensibility.Services.Logging;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility.Services;
using Swift.Extensibility;

namespace Swift.Modules.Services.Storage
{
    public class StorageManager : IStorageManager, IVaultManager, IInitializationAware, IPluginServiceUser, IShutdownAware
    {
        private ILoggingChannel _log;

        public StorageManager()
        {
            _log = ServiceLocator.Current.GetInstance<ILogger>().GetChannel<StorageManager>();
        }

        private DirectoryInfo _applicationDirectory = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
        public DirectoryInfo ApplicationDirectory
        {
            get { return _applicationDirectory; }
        }

        private DirectoryInfo _dataDirectory = new DirectoryInfo(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data"));
        public DirectoryInfo DataDirectory
        {
            get { return _dataDirectory; }
        }

        private DirectoryInfo _pluginsDirectory = new DirectoryInfo(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins"));
        public DirectoryInfo PluginsDirectory
        {
            get { return _pluginsDirectory; }
        }

        /// <summary>
        /// Gets the vault.
        /// </summary>
        public EncryptedStorageVault Vault { get; private set; }

        #region Initialization and Shutdown

        private IPluginServices _pluginServices;

        public void SetPluginServices(IPluginServices pluginServices)
        {
            _pluginServices = pluginServices;
        }

        public int InitializationPriority
        {
            get { return InternalConstants.InitializationPriorities.StorageManager; }
        }

        public void OnInitialization(InitializationEventArgs args)
        {
            if (!DataDirectory.Exists) DataDirectory.Create();
            if (!PluginsDirectory.Exists) PluginsDirectory.Create();

            try
            {
                Vault = EncryptedStorageVault.Open(Path.Combine(DataDirectory.FullName, "data.esv"), _pluginServices.GetCurrentUser().KeyMaterial, true, false);
                _log.Log("Vault opened.");
            }
            catch (Exception ex)
            {
                _log.Log(ex.GetType().Name + " occurred while trying to load storage vault. (" + ex.Message + ")");
                Vault = new EncryptedStorageVault();
            }
        }

        public int ShutdownPriority
        {
            get { return InternalConstants.ShutdownPriorities.StorageManager; }
        }

        public void OnShutdown(ShutdownEventArgs args)
        {
            if (Vault != null)
                Vault.Close(Path.Combine(DataDirectory.FullName, "data.esv"), _pluginServices.GetCurrentUser().KeyMaterial, true);
        }

        #endregion
    }
}
