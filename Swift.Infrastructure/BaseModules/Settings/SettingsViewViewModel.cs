using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using Swift.Extensibility.Input.Functions;
using Swift.Extensibility.Services;
using Swift.Extensibility.Services.Settings;
using Swift.Extensibility.UI;

namespace Swift.Infrastructure.BaseModules.Settings
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsViewViewModel : ViewModelBase, IInitializationAware, ISwiftFunctionSource
    {
        #region Properties

        private IEnumerable<SettingsSourceViewModel> _settingsSources;
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        private IEnumerable<SettingsSourceViewModel> SettingsSources => _settingsSources ?? (_settingsSources = _pluginServices.GetServices<ISettingsSource>().Select(_ => new SettingsSourceViewModel(_)));

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

        [Import]
        private IPluginServices _pluginServices;

        /// <summary>
        /// Called when the settings menuitem was clicked.
        /// </summary>
        [SwiftFunction("settings.show", CallMode = FunctionCallMode.Explicit)]
        public void OnSettingsMenuClicked()
        {
            _pluginServices.GetService<IUiService>().Navigate(this, NavigationTargets.CenterView);
        }

        #region IInitializationAware Implementation

        public int InitializationPriority => 0;

        public void OnInitialization(InitializationEventArgs args)
        {
            _pluginServices.GetService<IUiService>().AddUiResource(new Uri("pack://application:,,,/Swift.Infrastructure;component/BaseModules/Settings/SettingsViewTemplates.xaml", UriKind.Absolute));
            _pluginServices.GetService<IUiService>().RegisterMenuItem(new MenuItem("Show Settings", "settings.show", MenuTarget.TopBar, new Uri("pack://application:,,,/Swift;component/Images/im_settings.png")));
        }

        #endregion
    }
}
