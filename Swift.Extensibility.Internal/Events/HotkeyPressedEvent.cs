using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Swift.Extensibility.Events
{
    /// <summary>
    /// Represents the HotkeyPressed event.
    /// </summary>
    public class HotkeyPressedEvent : PubSubEvent<HotkeyPressedEventArgs> { }

    /// <summary>
    /// EventArgs for the HotkeyPressed event.
    /// </summary>
    public class HotkeyPressedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the ID of the hotkey that has been pressed.
        /// </summary>
        public string HotkeyId { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyPressedEventArgs"/> class.
        /// </summary>
        /// <param name="hotkeyId">The hotkey identifier.</param>
        public HotkeyPressedEventArgs(string hotkeyId)
        {
            HotkeyId = hotkeyId;
        }
    }

}
