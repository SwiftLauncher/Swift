﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Swift.Extensibility.Attributes;
using Swift.Extensibility.Logging;
using Swift.Extensibility.Services.Logging;
using Swift.Extensibility.Services.Settings;

namespace Swift.Infrastructure.Logging
{
    /// <summary>
    /// Implements ILogger, ILoggingManager.
    /// </summary>
    [Export(typeof(ILogger))]
    [SwiftSettingsNode]
    public class SwiftLogger : ILogger, ILoggingManager, ISettingsSource
    {
        private readonly IList<ILoggingChannel> _channels = new List<ILoggingChannel>();
        /// <summary>
        /// Gets the channels.
        /// </summary>
        public IEnumerable<ILoggingChannel> Channels => _channels;

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
            if (_channels.Any(_ => _.Id == channelName))
            {
                return _channels.First(_ => _.Id == channelName);
            }
            var l = new SwiftLoggingChannel(channelName);
            _channels.Add(l);
            l.MessageAdded += _ => OnMessageAdded(_.Message, _.ChannelId);
            OnChannelAdded(channelName);
            return l;
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <typeparam name="T">The requesting type.</typeparam>
        /// <returns>The LoggingChannel for the type.</returns>
        public ILoggingChannel GetChannel<T>() => GetChannel(typeof(T).FullName);

        /// <summary>
        /// Called when the MessageAdded event should be fired.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channelId">The channel identifier.</param>
        private void OnMessageAdded(LogMessage message, string channelId)
        {
            var m = MessageAdded;
            m?.Invoke(new MessageAddedEventArgs(message, channelId));
#if DEBUG
            System.Console.WriteLine(message.Message);
#endif
        }

        /// <summary>
        /// Called when a channel is added.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        private void OnChannelAdded(string channelId)
        {
            ChannelAdded?.Invoke(new ChannelAddedEventArgs(channelId));
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

        public string DisplayName => "Logging";

        public ISettingsSource Parent => null;

        private void PrepareSettings()
        {
            var sl = new List<ISetting>
            {
                new Header("Test Header (WOOOOOH :D)")
            };
            // TODO change to actual settings
            Settings = sl;
        }

        #endregion


    }
}
