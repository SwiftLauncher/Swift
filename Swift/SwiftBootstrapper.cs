using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility;
using Swift.Extensibility.Input;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Services;
using Swift.Extensibility.Services.Logging;
using Swift.Views;
using System.Linq;
using Swift.AppServices.Extensibility;
using Swift.ViewModels;

namespace Swift
{
    /// <summary>
    /// The bootstrapper for Swift.
    /// </summary>
    public class SwiftBootstrapper : MefBootstrapper
    {
        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>
        /// The shell of the application.
        /// </returns>
        /// <remarks>
        /// If the returned instance is a <see cref="T:System.Windows.DependencyObject" />, the
        /// <see cref="T:Microsoft.Practices.Prism.Bootstrapper" /> will attach the default <see cref="T:Microsoft.Practices.Prism.Regions.IRegionManager" /> of
        /// the application in its <see cref="F:Microsoft.Practices.Prism.Regions.RegionManager.RegionManagerProperty" /> attached property
        /// in order to be able to add regions by using the <see cref="F:Microsoft.Practices.Prism.Regions.RegionManager.RegionNameProperty" />
        /// attached property from XAML.
        /// </remarks>
        protected override DependencyObject CreateShell()
        {
            var sh = ServiceLocator.Current.GetInstance<SwiftShell>();
            sh.DataContext = ServiceLocator.Current.GetInstance<ShellViewModel>();
            return sh;
        }

        /// <summary>
        /// Configures the <see cref="P:Microsoft.Practices.Prism.MefExtensions.MefBootstrapper.AggregateCatalog" /> used by MEF.
        /// </summary>
        /// <returns>
        /// An <see cref="P:Microsoft.Practices.Prism.MefExtensions.MefBootstrapper.AggregateCatalog" /> to be used by the bootstrapper.
        /// </returns>
        /// <remarks>
        /// The base implementation returns a new AggregateCatalog.
        /// </remarks>
        protected override AggregateCatalog CreateAggregateCatalog()
        {
            return new ConfigurationBasedFilteringCatalog();
        }

        /// <summary>
        /// Configures the <see cref="P:Microsoft.Practices.Prism.MefExtensions.MefBootstrapper.AggregateCatalog" /> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        protected override void ConfigureAggregateCatalog()
        {
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(SwiftShell).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Swift.AppServices.Events.EventBroker).Assembly));

            var speckey = @"0024000004800000940000000602000000240000525341310004000001000100d7042bf2942022d5a3d83204c1718c9fc2904f8a25795c8037461a53bc49ec84587b870bc39b322b0531dfd4d10b718ed0663b6eb7b05e3710847f59524fa1c04dec34d1cd50115794f31c00031e75822b81987610116e23993c92ec5efe91016c4185cc843664f26319ada3613616d8eb00a174f8b29714612d48d6bff9a7d9";

            var spec = new List<byte>(speckey.Length / 2);

            for (int i = 0; i < speckey.Length - 1; i += 2)
            {
                string b = "" + speckey[i] + speckey[i + 1];
                spec.Add(byte.Parse(b, NumberStyles.HexNumber));
            }

            var swiftroot = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            var tpc = new TrustedPluginCatalog(Path.Combine(swiftroot, "Plugins"), spec.ToArray(), typeof(SwiftShell).Assembly.GetName().GetPublicKey());
            this.AggregateCatalog.Catalogs.Add(tpc);
        }

        /// <summary>
        /// Creates the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> that will be used as the default container.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" />.
        /// </returns>
        /// <remarks>
        /// The base implementation registers a default MEF catalog of exports of key Prism types.
        /// Exporting your own types will replace these defaults.
        /// </remarks>
        protected override CompositionContainer CreateContainer()
        {
            return new CompositionContainer(this.AggregateCatalog);
        }

        /// <summary>
        /// Configures the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" />.
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        /// <remarks>
        /// The base implementation registers all the types direct instantiated by the bootstrapper with the container.
        /// If the method is overwritten, the new implementation should call the base class version.
        /// </remarks>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
        }

        /// <summary>
        /// Creates the <see cref="T:Microsoft.Practices.Prism.Modularity.IModuleCatalog" /> used by Prism.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The base implementation returns a new ModuleCatalog.
        /// </remarks>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            // TODO load modulecatalog
            return base.CreateModuleCatalog();
        }

        /// <summary>
        /// Configures the <see cref="T:Microsoft.Practices.Prism.Modularity.IModuleCatalog" /> used by Prism.
        /// </summary>
        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        /// <remarks>
        /// The base implementation ensures the shell is composed in the container.
        /// </remarks>
        protected override async void InitializeShell()
        {
            var lc = ServiceLocator.Current.GetInstance<ILogger>().GetChannel<SwiftBootstrapper>();
            lc.Log("In InitializeShell");

            // LOGIN
            var success = false;
            IUserProfile userProfile = null;
            while (!success)
            {
                ILoginDialog loginDialog = null;
                try
                {
                    loginDialog = ServiceLocator.Current.GetInstance<ILoginDialog>();
                }
                catch
                {
                    MessageBox.Show("Swift could not find required module 'LoginDialog'. You may have to reinstall Swift to resolve this problem.");
                    Application.Current.Shutdown();
                }
                var result = loginDialog.ShowLoginDialog();
                switch (result)
                {
                    case LoginResult.Successful:
                        success = true;
                        userProfile = loginDialog.UserProfile;
                        break;
                    case LoginResult.Failed:
                        success = false;
                        break;
                    case LoginResult.Aborted:
                        Application.Current.Shutdown();
                        return;
                    default:
                        break;
                }
            }

            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();

            // Initialization
            foreach (var iia in ServiceLocator.Current.GetAllInstances<IInitializationAware>().OrderBy(_ => _.InitializationPriority))
            {
                iia.OnInitialization(new InitializationEventArgs());
            }
        }

        internal void Shutdown()
        {
            // Shutdown
            foreach (var isa in ServiceLocator.Current.GetAllInstances<IShutdownAware>().OrderBy(_ => _.ShutdownPriority))
            {
                isa.OnShutdown(new ShutdownEventArgs());
            }
        }
    }
}
