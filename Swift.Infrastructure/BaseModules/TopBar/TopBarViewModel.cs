using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Swift.Extensibility;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Services;
using Swift.Extensibility.UI;
using Swift.Toolkit;

namespace Swift.Infrastructure.BaseModules.TopBar
{
    /// <summary>
    /// The viewmodel for Swift's top bar.
    /// </summary>
    [Export]
    public class TopBarViewModel : ViewModelBase, IInitializationAware, IPluginServiceUser
    {
        #region Properties

        /// <summary>
        /// Backing field for <see cref="InputBoxViewModel"/>.
        /// </summary>
        private object _inputBoxViewModel;
        /// <summary>
        /// Gets or sets the input box view model.
        /// </summary>
        /// <value>
        /// The input box view model.
        /// </value>
        [NavigationTarget(ViewTargetsInternal.InputBoxPlaceHolder)]
        public object InputBoxViewModel { get { return _inputBoxViewModel; } set { Set(ref _inputBoxViewModel, value); } }

        private SyncedViewModelCollection<MenuItem, MenuItemViewModel> _menuItems;
        public IEnumerable<MenuItemViewModel> MenuItems
        {
            get
            {
                return _menuItems ?? (_menuItems = new SyncedViewModelCollection<MenuItem, MenuItemViewModel>(_pluginManager.MenuItems, _ => new MenuItemViewModel(_), _ => _.Model));
            }
        }

        #endregion

        /// <summary>
        /// The plugin manager
        /// </summary>
        [Import]
        private IPluginManager _pluginManager;
        private IPluginServices _pluginServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopBarViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public TopBarViewModel()
        {

        }

        #region IInitializationAware Implementation

        public int InitializationPriority => -1;

        public void OnInitialization(InitializationEventArgs args)
        {
            _pluginServices.GetService<IUiService>().AddUiResource(new Uri("pack://application:,,,/Swift.Infrastructure;component/BaseModules/TopBar/TopBarTemplates.xaml", UriKind.Absolute));
            _pluginServices.GetService<IUiService>().Navigate(this, ViewTargetsInternal.TopBar);
        }

        #endregion

        public void SetPluginServices(IPluginServices pluginServices)
        {
            _pluginServices = pluginServices;
        }
    }
}
