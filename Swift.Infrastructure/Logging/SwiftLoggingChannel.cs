using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Swift.Extensibility.Services.Logging;

namespace Swift.Infrastructure.Logging
{
    /// <summary>
    /// Implements ILoggingChannel.
    /// </summary>
    public class SwiftLoggingChannel : ILoggingChannel
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; }

        private readonly IList<LogMessage> _messages = new List<LogMessage>();
        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public IEnumerable<LogMessage> Messages => _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwiftLoggingChannel"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public SwiftLoggingChannel(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="sourceLine">The source line.</param>
        public void Log(string message, LogMessageSeverity severity = LogMessageSeverity.Output, [CallerMemberName] string sender = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int sourceLine = 0)
        {
            var lm = new LogMessage(Id, message, severity, sender, sourceFile, sourceLine);
            _messages.Add(lm);
            OnMessageAdded(lm);
        }

        /// <summary>
        /// Called when the MessageAdded event should be fired.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessageAdded(LogMessage message)
        {
            MessageAdded?.Invoke(new MessageAddedEventArgs(message, Id));
        }

        /// <summary>
        /// Occurs when a message is added.
        /// </summary>
        public event MessageAddedEventHandler MessageAdded;
    }
}
