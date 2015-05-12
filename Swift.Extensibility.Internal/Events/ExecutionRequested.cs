using System;
using Microsoft.Practices.Prism.PubSubEvents;
using Swift.Extensibility.Input;

namespace Swift.Extensibility.Events
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
        public ExecutionType ExecutionType { get; private set; }

        /// <summary>
        /// Gets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        public IInput Input { get; private set; }

        /// <summary>
        /// Gets the execution callback.
        /// </summary>
        public Action ExecutionCallback { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="args">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        /// <param name="input">The input.</param>
        public ExecutionRequestedEventArgs(ExecutionType executionType, IInput input, Action executionCallback = null)
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
