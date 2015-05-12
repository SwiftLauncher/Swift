using System;

namespace Swift.Extensibility.Services.Logging
{
    /// <summary>
    /// Implements ILogMessage.
    /// </summary>
    public class LogMessage : ILogMessage
    {
        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        public LogMessageSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the source file.
        /// </summary>
        public string SourceFile { get; set; }

        /// <summary>
        /// Gets or sets the source line.
        /// </summary>
        public int SourceLine { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        private LogMessage(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }

        /// <summary>
        /// Creates the specified channel identifier.
        /// </summary>
        /// <param name="channelID">The channel identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="sourceLine">The source line.</param>
        /// <returns></returns>
        public static ILogMessage Create(string channelID, string sender, string message, LogMessageSeverity severity, string sourceFile, int sourceLine)
        {
            var lm = new LogMessage(sender, message);
            lm.ChannelID = channelID;
            lm.Severity = severity;
            lm.SourceFile = sourceFile;
            lm.SourceLine = sourceLine;
            return lm;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "" + ChannelID + (Severity == LogMessageSeverity.Output ? "" : " [" + Enum.GetName(typeof(LogMessageSeverity), Severity) + "]") + ": " + Message + "(@" + SourceFile + ":" + SourceLine + ")";
        }
    }
}
