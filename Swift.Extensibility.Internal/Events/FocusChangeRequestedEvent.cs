using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Swift.Extensibility.Events
{
    /// <summary>
    /// Represents the FocusChangeRequested event.
    /// </summary>
    public class FocusChangeRequestedEvent : PubSubEvent<FocusChangeRequestedEventArgs> { }

    /// <summary>
    /// EventArgs for the FocusChangeRequested event.
    /// </summary>
    public class FocusChangeRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Target to be focused.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public string Target { get; private set; }

        /// <summary>
        /// Gets or sets the type of the change.
        /// </summary>
        /// <value>
        /// The type of the change.
        /// </value>
        public FocusChangeType ChangeType { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FocusChangeRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="target">The target to be focused.</param>
        public FocusChangeRequestedEventArgs(string target, FocusChangeType changeType = FocusChangeType.Default)
            : base()
        {
            Target = target;
            ChangeType = changeType;
        }
    }

    /// <summary>
    /// The FocusChange types.
    /// </summary>
    public enum FocusChangeType
    {
        Default = 0,
        Start = 1,
        End = 2,
        Select = 3
    }

}
