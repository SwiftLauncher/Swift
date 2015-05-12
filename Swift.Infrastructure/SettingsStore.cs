using System;
using System.Collections.Generic;
using Swift.Extensibility.Services.Settings;
using System.Linq;

namespace Swift.Modules.Services
{
    [Serializable]
    public class SettingsStore : ISettingsStore
    {
        private static IEnumerable<Type> _storableTypes = new[] { typeof(char), typeof(string), typeof(byte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double) };
        /// <summary>
        /// The storable types.
        /// </summary>
        public IEnumerable<Type> StorableTypes
        {
            get
            {
                return _storableTypes;
            }
        }

        private Dictionary<string, object> _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsStore"/> class.
        /// </summary>
        public SettingsStore()
        {
            _store = new Dictionary<string, object>();
        }

        /// <summary>
        /// Clears the listeners.
        /// </summary>
        public void ClearListeners()
        {
            EntryChanged = null;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="System.ArgumentException">The type ' + value.GetType().FullName + ' cannot be stored in this ISettingsStore. Storable types are: + StorableTypes.Aggregate(, (s, t) => s +  ' + t.Name + ')</exception>
        public object this[string key]
        {
            get
            {
                if (_store.ContainsKey(key))
                    return _store[key];
                else
                    throw new KeyNotFoundException();
            }
            set
            {
                if (!StorableTypes.Any(_ => _ == value.GetType())) throw new ArgumentException("The type '" + value.GetType().FullName + "' cannot be stored in this ISettingsStore. Storable types are:" + StorableTypes.Aggregate("", (s, t) => s + " '" + t.Name + "'"));
                object oldValue = null;
                if (_store.ContainsKey(key))
                {
                    oldValue = _store[key];
                    _store[key] = value;
                }
                else
                {
                    _store.Add(key, value);
                }
                var e = EntryChanged;
                if (e != null)
                    e(this, new SettingsStoreEntryChangedEventArgs(key, oldValue, value));
            }
        }

        /// <summary>
        /// Retrieves the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Retrieve<T>(string key, T defaultValue = default(T))
        {
            try
            {
                return (T)_store[key];
            }
            catch
            {
                _store[key] = defaultValue;
                return defaultValue;
            }
        }

        /// <summary>
        /// Stores the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Store<T>(string key, T value)
        {
            _store[key] = value;
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _store.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the specified value contains value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool ContainsValue(object value)
        {
            return _store.ContainsValue(value);
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool DeleteEntry(string key)
        {
            if (_store.ContainsKey(key))
            {
                var oldval = _store[key];
                _store.Remove(key);
                var e = EntryChanged;
                if (e != null)
                    e(this, new SettingsStoreEntryChangedEventArgs(key, oldval, null));
            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Occurs when [entry changed].
        /// </summary>
        public event SettingsStoreEntryChangedEventHandler EntryChanged;
    }
}
