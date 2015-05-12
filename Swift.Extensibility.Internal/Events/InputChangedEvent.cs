using System;
using Microsoft.Practices.Prism.PubSubEvents;
using Swift.Extensibility.Input;

namespace Swift.Extensibility.Events
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
        public IInput NewInput { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="InputChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newInput">The new input.</param>
        public InputChangedEventArgs(IInput newInput)
            : base()
        {
            NewInput = newInput;
        }
    }

}
