using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Swift.Extensibility.Services.Logging;

namespace Swift.Extensibility.Logging
{
    /// <summary>
    /// Interface for LoggingManagers.
    /// </summary>
    [InheritedExport]
    public interface ILoggingManager
    {
        /// <summary>
        /// Gets the channels.
        /// </summary>
        IEnumerable<ILoggingChannel> Channels { get; }

        /// <summary>
        /// Occurs when a message is added to any of the channels.
        /// </summary>
        event MessageAddedEventHandler MessageAdded;

        /// <summary>
        /// Occurs when a channel is added.
        /// </summary>
        event ChannelAddedEventHandler ChannelAdded;
    }

    /// <summary>
    /// Event handler for channel added events.
    /// </summary>
    /// <param name="args">The <see cref="ChannelAddedEventArgs"/> instance containing the event data.</param>
    public delegate void ChannelAddedEventHandler(ChannelAddedEventArgs args);

    /// <summary>
    /// Event args for channel added events.
    /// </summary>
    public class ChannelAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        public string ChannelID { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelAddedEventArgs"/> class.
        /// </summary>
        /// <param name="channelID">The channel identifier.</param>
        public ChannelAddedEventArgs(string channelID)
        {
            ChannelID = channelID;
        }
    }
}
