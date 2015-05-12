using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift.Extensibility.Events;

namespace Swift.AppServices.Events
{
    /// <summary>
    /// Implementation of IEventBroker for Swift.
    /// </summary>
    [Export(typeof(IEventBroker))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EventBroker : IEventBroker
    {
        /// <summary>
        /// List of currently instantiated channels.
        /// </summary>
        private List<object> _channels = new List<object>();

        /// <summary>
        /// Gets the channel with the given name and argument type. If no such channel exists, it will be created.
        /// </summary>
        /// <typeparam name="T">The type of the event arguments</typeparam>
        /// <param name="name">The name of the channel to get.</param>
        /// <returns>
        /// A reference to the IEventChannel with the given name and argument type.
        /// </returns>
        public IEventChannel<T> GetChannel<T>(string name)
        {
            var c = _channels.OfType<EventChannel<T>>().FirstOrDefault(_ => !_.Typeless && _.Name == name);
            if (c == null)
            {
                c = new EventChannel<T>(name, false);
                _channels.Add(c);
            }
            return c;
        }

        /// <summary>
        /// Gets the channel with the given name. If no such channel exists, it will be created.
        /// </summary>
        /// <param name="name">The name of the channel to get.</param>
        /// <returns>
        /// A reference to the IEventChannel with the given name.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEventChannel GetChannel(string name)
        {
            var c = _channels.OfType<EventChannel<object>>().FirstOrDefault(_ => _.Typeless && _.Name == name);
            if (c == null)
            {
                c = new EventChannel<object>(name, true);
                _channels.Add(c);
            }
            return c;
        }
    }
}
