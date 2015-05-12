using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Swift.Extensibility.Events;

namespace Swift.Infrastructure.Events
{
    /// <summary>
    /// Swift implementation of <see cref="IEventChannel"/>.
    /// </summary>
    /// <typeparam name="T">Type of the event arguments. Can be anything if only used as the non-generic channel.</typeparam>
    public class EventChannel<T> : IEventChannel, IEventChannel<T>
    {
        /// <summary>
        /// Gets the name of this channel.
        /// </summary>
        /// <value>
        /// The name of this channel.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EventChannel{T}"/> is typeless.
        /// </summary>
        /// <value>
        ///   <c>true</c> if typeless; otherwise, <c>false</c>.
        /// </value>
        public bool Typeless { get; private set; }

        /// <summary>
        /// The subject.
        /// </summary>
        private Subject<T> _subject = new Subject<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventChannel{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public EventChannel(string name, bool typeless)
        {
            Name = name;
            if (typeless && typeof(T) != typeof(object))
                throw new ArgumentException("Type of EventChannels marked as typeless has to be object.");
            Typeless = typeless;
        }

        /// <summary>
        /// Subscribes to this channel using the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>
        /// An IDisposable that will unsubscribe the given handler when disposed.
        /// </returns>
        public IDisposable Subscribe(Action handler)
        {
            return _subject.Subscribe(_ => handler());
        }

        /// <summary>
        /// Subscribes the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        public IDisposable Subscribe(Action<T> handler)
        {
            return _subject.Subscribe(handler);
        }

        /// <summary>
        /// Fires an event on this channel.
        /// </summary>
        public void Publish()
        {
            _subject.OnNext(default(T));
        }

        /// <summary>
        /// Publishes the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public void Publish(T arguments)
        {
            _subject.OnNext(arguments);
        }
    }
}
