using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Swift.Extensibility.Internal.Events
{
    /// <summary>
    /// Represents the ExecutionRequested event.
    /// </summary>
    public class ExecutionRequestedEvent : PubSubEvent<ExecutionRequestedEventArgs> { }

    /// <summary>
    /// EventArgs for the ExecutionRequested event.
    /// </summary>
    public class ExecutionRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the key event arguments.
        /// </summary>
        /// <value>
        /// The key event arguments.
        /// </value>
        public ExecutionType ExecutionType { get; }

        /// <summary>
        /// Gets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        public Input.Input Input { get; }

        /// <summary>
        /// Gets the execution callback.
        /// </summary>
        public Action ExecutionCallback { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="args">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        /// <param name="input">The input.</param>
        public ExecutionRequestedEventArgs(ExecutionType executionType, Input.Input input, Action executionCallback = null)
            : base()
        {
            ExecutionType = executionType;
            Input = input;
            ExecutionCallback = executionCallback;
        }
    }

    public enum ExecutionType
    {
        Default,
        HideBeforeExecution
    }
}
