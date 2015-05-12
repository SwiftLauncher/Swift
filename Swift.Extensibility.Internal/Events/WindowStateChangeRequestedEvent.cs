using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Swift.Extensibility.Events
{
    public class WindowStateChangeRequestedEvent : PubSubEvent<WindowStateChangeRequestedEventArgs> { }

    public class WindowStateChangeRequestedEventArgs : EventArgs
    {
        public WindowState TargetState { get; private set; }

        public WindowStateChangeRequestedEventArgs(WindowState targetState)
            : base()
        {
            TargetState = targetState;
        }
    }

    public enum WindowState
    {
        Shown,
        Hidden,
        Toggle,
        Foreground
    }
}
