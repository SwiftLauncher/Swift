using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Swift.Extensibility.Logging;
using Swift.Extensibility.Services.Logging;
using Swift.Extensibility.Services.Settings;

namespace Swift.Infrastructure.BaseModules.Logging
{
    /// <summary>
    /// Implements ILogger, ILoggingManager.
    /// </summary>
    public class SwiftLogger : ILogger, ILoggingManager, ISettingsSource
    {
        private IList<ILoggingChannel> _channels = new List<ILoggingChannel>();
        /// <summary>
        /// Gets the channels.
        /// </summary>
        public IEnumerable<ILoggingChannel> Channels { get { return _channels; } }

        private ILoggingChannel _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwiftLogger"/> class.
        /// </summary>
        [ImportingConstructor]
        public SwiftLogger()
        {
            PrepareSettings();
            _log = GetChannel("Log System");
            _log.Log("SwiftLogger created.");
        }

        /// <summary>
        /// Gets the channel with the given name.
        /// </summary>
        /// <param name="channelName">Name of the channel.</param>
        /// <returns>The LoggingChannel with the given name.</returns>
        public ILoggingChannel GetChannel(string channelName)
        {
            if (_channels.Any(_ => _.ID == channelName))
            {
                return _channels.First(_ => _.ID == channelName);
            }
            else
            {
                var l = new SwiftLoggingChannel(channelName);
                _channels.Add(l);
                l.MessageAdded += _ => OnMessageAdded(_.Message, _.ChannelID);
                OnChannelAdded(channelName);
                return l;
            }
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <typeparam name="T">The requesting type.</typeparam>
        /// <returns>The LoggingChannel for the type.</returns>
        public ILoggingChannel GetChannel<T>()
        {
            return GetChannel(typeof(T).FullName);
        }

        /// <summary>
        /// Called when the MessageAdded event should be fired.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessageAdded(ILogMessage message, string channelID)
        {
            var m = MessageAdded;
            if (m != null)
            {
                m(new MessageAddedEventArgs(message, channelID));
            }
#if DEBUG
            System.Console.WriteLine(message.Message);
#endif
        }

        /// <summary>
        /// Called when a channel is added.
        /// </summary>
        /// <param name="channelID">The channel identifier.</param>
        private void OnChannelAdded(string channelID)
        {
            var c = ChannelAdded;
            if (c != null)
            {
                c(new ChannelAddedEventArgs(channelID));
            }
        }

        /// <summary>
        /// Occurs when a message is added to any of the channels.
        /// </summary>
        public event MessageAddedEventHandler MessageAdded;

        /// <summary>
        /// Occurs when a channel is added.
        /// </summary>
        public event ChannelAddedEventHandler ChannelAdded;

        #region ISettingsSource Implementation

        public IEnumerable<ISetting> Settings { get; private set; }

        public string DisplayName
        {
            get { return "Logging"; }
        }

        public ISettingsSource Parent
        {
            get { return null; }
        }

        private void PrepareSettings()
        {
            var sl = new List<ISetting>();
            // TODO change to actual settings
            sl.Add(new Header("Test Header (WOOOOOH :D)"));
            sl.Add(new BooleanSetting("Test boolean setting (Check this!)", false, _ => { System.Windows.MessageBox.Show("Value is now " + _); }, false, "Tooltiptest", "Really important setting"));
            sl.Add(new IntegerSetting("IntegerSetting (only 0-10)", 5, _ => { }, 5, "TT", "Description goes here", 0, 10));
            sl.Add(new CustomSetting("Test custom setting", new IntegerSetting("IntegerSetting (inside CUSTOM!!)", 5, _ => { }, 5, "TT", "Description goes here", 0, 10)));
            for (int i = 0; i < 40; i++)
            {
                sl.Add(new BooleanSetting("Test boolean setting (Check this!)", false, _ => { System.Windows.MessageBox.Show("Value is now " + _); }, false, "Tooltiptest", "Really important setting"));
            }
            Settings = sl;
        }

        #endregion


    }
}
