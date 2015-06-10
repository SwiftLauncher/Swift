using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Swift.Extensibility.Internal.Events
{
    /// <summary>
    /// Represents the InputChanged event.
    /// </summary>
    public class InputChangedEvent : PubSubEvent<InputChangedEventArgs> { }

    /// <summary>
    /// EventArgs for the InputChanged event.
    /// </summary>
    public class InputChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the new Input.
        /// </summary>
        public Input.Input NewInput { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="InputChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newInput">The new input.</param>
        public InputChangedEventArgs(Input.Input newInput)
        {
            NewInput = newInput;
        }
    }

}
