using System;
using System.ComponentModel.Composition;
using System.IO;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Services;
using Swift.Extensibility.Services.Logging;
using Swift.Extensibility.Services.Storage;
using Swift.Toolkit.Data;

namespace Swift.Infrastructure.Storage
{
    [Export(typeof(IStorageManager))]
    public class StorageManager : IStorageManager, IVaultManager, IInitializationAware, IShutdownAware
    {
        private ILoggingChannel _log;
        [Import]
        private IPluginServices _pluginServices;

        [ImportingConstructor]
        public StorageManager(ILogger logger)
        {
            _log = logger.GetChannel<StorageManager>();
        }

        public DirectoryInfo ApplicationDirectory { get; } = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        public DirectoryInfo DataDirectory { get; } = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));

        public DirectoryInfo PluginsDirectory { get; } = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));

        /// <summary>
        /// Gets the vault.
        /// </summary>
        public EncryptedStorageVault Vault { get; private set; }

        public int InitializationPriority => InternalConstants.InitializationPriorities.StorageManager;

        public void OnInitialization(InitializationEventArgs args)
        {
            if (!DataDirectory.Exists) DataDirectory.Create();
            if (!PluginsDirectory.Exists) PluginsDirectory.Create();

            try
            {
                Vault = EncryptedStorageVault.Open(Path.Combine(DataDirectory.FullName, "data.esv"), Convert.ToBase64String(_pluginServices.GetCurrentUser().KeyMaterial), true);
                _log.Log("Vault opened.");
            }
            catch (Exception ex)
            {
                _log.Log(ex.GetType().Name + " occurred while trying to load storage vault. (" + ex.Message + ")");
                Vault = new EncryptedStorageVault();
            }
        }

        public int ShutdownPriority => InternalConstants.ShutdownPriorities.StorageManager;

        public void OnShutdown(ShutdownEventArgs args)
        {
            Vault?.Close(Path.Combine(DataDirectory.FullName, "data.esv"), Convert.ToBase64String(_pluginServices.GetCurrentUser().KeyMaterial), true);
        }
    }
}
