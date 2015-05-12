using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Swift.Extensibility.Attributes;
using Swift.Extensibility.Services.Settings;
using Swift.Extensibility.UI;
using Swift.Toolkit;

namespace Swift.AppServices.BaseModules
{
    public sealed class SettingsSourceViewModel : BindableBase
    {
        #region Properties

        private ISettingsSource _source;
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public ISettingsSource Source
        {
            get { return _source; }
            set { Set(ref _source, value); }
        }

        private string _settingsGroup;
        /// <summary>
        /// Gets the settings group.
        /// </summary>
        /// <value>
        /// The settings group.
        /// </value>
        public string SettingsGroup
        {
            get { return _settingsGroup; }
            private set { Set(ref _settingsGroup, value); }
        }

        private string _settingsSubGroup;
        /// <summary>
        /// Gets the settings sub group.
        /// </summary>
        /// <value>
        /// The settings sub group.
        /// </value>
        public string SettingsSubGroup
        {
            get { return _settingsSubGroup; }
            private set { Set(ref _settingsSubGroup, value); }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName
        {
            get { return Source.DisplayName; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsSourceViewModel"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public SettingsSourceViewModel(ISettingsSource source)
        {
            Source = source;
            if (source.GetType().GetCustomAttributes(typeof(SwiftSettingsNodeAttribute), false) != null)
            {
                SettingsGroup = "Swift Settings";
            }
            else
            {
                SettingsGroup = "Plugin Settings";
            }
            // TODO handle hierarchy
        }
    }
}
