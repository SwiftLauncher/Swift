using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using Swift.Extensibility;
using Swift.Extensibility.Services;
using Swift.Extensibility.Services.Settings;
using Swift.Extensibility.UI;

namespace Swift.Infrastructure.BaseModules
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsViewViewModel : ViewModelBase, IInitializationAware, IPluginServiceUser
    {
        #region Properties

        private IEnumerable<SettingsSourceViewModel> _settingsSources;
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        private IEnumerable<SettingsSourceViewModel> SettingsSources
        {
            get
            {
                return _settingsSources ?? (_settingsSources = _pluginServices.GetServices<ISettingsSource>().Select(_ => new SettingsSourceViewModel(_)));
            }
        }

        private ICollectionView _settingsSourcesCollectionView;
        /// <summary>
        /// Gets the settings collection view.
        /// </summary>
        /// <value>
        /// The settings collection view.
        /// </value>
        public ICollectionView SettingsSourcesCollectionView
        {
            get
            {
                if (_settingsSourcesCollectionView == null)
                {
                    _settingsSourcesCollectionView = CollectionViewSource.GetDefaultView(SettingsSources);
                    _settingsSourcesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("SettingsGroup"));
                    _settingsSourcesCollectionView.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
                }
                return _settingsSourcesCollectionView;
            }
        }

        #endregion

        private IPluginServices _pluginServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewViewModel"/> class.
        /// </summary>
        public SettingsViewViewModel()
        {

        }

        /// <summary>
        /// Called when the settings menuitem was clicked.
        /// </summary>
        private void OnSettingsMenuClicked()
        {
            _pluginServices.GetService<IUIService>().Navigate(this, NavigationTargets.CenterView);
        }

        #region IInitializationAware Implementation

        public int InitializationPriority
        {
            get { return 0; }
        }

        public void OnInitialization(InitializationEventArgs args)
        {
            _pluginServices.GetService<IUIService>().AddUIResource(new Uri("pack://application:,,,/Swift.Infrastructure;component/BaseModules/Settings/SettingsViewTemplates.xaml", UriKind.Absolute));
            _pluginServices.GetService<IUIService>().RegisterMenuItem(new MenuItem("Show Settings", OnSettingsMenuClicked, MenuTarget.TopBar, new Uri("pack://application:,,,/Swift;component/Images/im_settings.png")));
        }

        #endregion

        public void SetPluginServices(IPluginServices pluginServices)
        {
            _pluginServices = pluginServices;
        }
    }
}
