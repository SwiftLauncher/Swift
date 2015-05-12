using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Swift.Toolkit.Data
{
    /// <summary>
    /// A class for securely storing arbitrary data (encrypted with TripleDES and a password-derived key).
    /// </summary>
    public class EncryptedStorageVault
    {
        /// <summary>
        /// The internal Dictionary used for storing the items.
        /// </summary>
        private Dictionary<string, object> _store = null;

        /// <summary>
        /// Creates a new empty <see cref="EncryptedStorageVault"/>.
        /// </summary>
        public EncryptedStorageVault()
        {
            _store = new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates an <see cref="EncryptedStorageVault"/> with the given Dictionary as the public store.
        /// </summary>
        /// <param name="store">The Dictionary (not necessarily empty) to be used as the public store.</param>
        public EncryptedStorageVault(Dictionary<string, object> store)
        {
            _store = store;
        }

        /// <summary>
        /// Retrieves the item with the given key from this <see cref="EncryptedStorageVault"/>.
        /// </summary>
        /// <typeparam name="T">The type of the stored item.</typeparam>
        /// <param name="key">The key of the item to be retrieved.</param>
        /// <returns>The item with the given key.</returns>
        public T Retrieve<T>(string key)
        {
            if (!_store.ContainsKey(key))
                throw new KeyNotFoundException("Key '" + key + "' was not found in this store.");
            return (T)_store[key];
        }

        /// <summary>
        /// Tries to retrieve the item with the given key from this <see cref="EncryptedStorageVault"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item to be retrieved.</typeparam>
        /// <param name="key">The key of the item to be retrieved.</param>
        /// <param name="value">The item with the given key, if it could be retrieved successfully. Otherwise the default value of type T.</param>
        /// <returns>True, if the retrieval was successful. False otherwise.</returns>
        public bool TryRetrieve<T>(string key, out T value)
        {
            try
            {
                value = Retrieve<T>(key);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Stores an item with the given key in this <see cref="EncryptedStorageVault"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="key">The key to identify the item</param>
        /// <param name="value">The item to be stored.</param>
        /// <param name="overrideIfExisting">If false, trying to store an item with a key that is already used will result in an exception.</param>
        public void Store<T>(string key, T value, bool overrideIfExisting = true)
        {
            //if (!(typeof(T).GetCustomAttributes(typeof(SerializableAttribute), false) == null))
            //    throw new ArgumentException("Values that are stored have to be marked as Serializable");
            if (!_store.ContainsKey(key))
                _store.Add(key, value);
            else
            {
                if (overrideIfExisting)
                    _store[key] = value;
                else
                    throw new InvalidOperationException("An item with this key is already stored and overriding has been disabled.");
            }
        }

        /// <summary>
        /// Tries to store the given item with the given key in this <see cref="EncryptedStorageVault"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item to be stored.</typeparam>
        /// <param name="key">The key to identify the item.</param>
        /// <param name="value">The item to be stored.</param>
        /// <param name="overrideIfExisting">If false, trying to store an item with a key that is already used will not succeed.</param>
        /// <returns>True, if the item was successfully stored. False otherwise.</returns>
        public bool TryStore<T>(string key, T value, bool overrideIfExisting = true)
        {
            try
            {
                Store(key, value, overrideIfExisting);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Opens an existing <see cref="EncryptedStorageVault"/> from the given file.
        /// </summary>
        /// <param name="path">The path to the file that contains the <see cref="EncryptedStorageVault"/>.</param>
        /// <param name="password">The password for that <see cref="EncryptedStorageVault"/>.</param>
        /// <param name="createIfNotExisting">If true, a new <see cref="EncryptedStorageVault"/> will be created and returned when the given file can not be found.</param>
        /// <param name="createIfError">If true, a new <see cref="EncryptedStorageVault"/> will be created and returned if an error occurred while the given file was read.</param>
        /// <returns>The <see cref="EncryptedStorageVault"/>.</returns>
        public static EncryptedStorageVault Open(string path, string password, bool createIfNotExisting = false, bool createIfError = false)
        {
            if (!File.Exists(path))
            {
                if (createIfNotExisting)
                    return new EncryptedStorageVault();
                else
                    throw new FileNotFoundException("Could not find vault file '" + path + "'.");
            }

            try
            {
                var csp = new TripleDESCryptoServiceProvider();
                var key = KeyFromPassword(password);
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(fs, csp.CreateDecryptor(key, key), CryptoStreamMode.Read))
                    {
                        object o = bf.Deserialize(cs);
                        return new EncryptedStorageVault(o as Dictionary<string, object>);
                    }
                }
            }
            catch (Exception ex)
            {
                if (createIfError)
                {
                    return new EncryptedStorageVault();
                }
                else
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Closes this <see cref="EncryptedStorageVault"/> and saves it to the given file, protected with a key derived from the given password.
        /// </summary>
        /// <param name="path">The path of the file where this <see cref="EncryptedStorageVault"/> will be stored.</param>
        /// <param name="password">The password.</param>
        /// <param name="overrideExisting">If set to <c>true</c> and the given file already exists, it will be overridden.</param>
        public void Close(string path, string password, bool overrideExisting = false)
        {
            if (File.Exists(path) && !overrideExisting)
                throw new IOException("Storage file already exists.");

            var csp = new TripleDESCryptoServiceProvider();
            var key = KeyFromPassword(password);
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (CryptoStream cs = new CryptoStream(fs, csp.CreateEncryptor(key, key), CryptoStreamMode.Write))
                {
                    bf.Serialize(cs, _store);
                    cs.FlushFinalBlock();
                }
            }
        }

        /// <summary>
        /// Gets or sets the item with the specified key.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        /// <param name="key">The key of the item to be retrieved.</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                return Retrieve<object>(key);
            }
            set
            {
                Store(key, value);
            }
        }

        /// <summary>
        /// Derives a key from the given password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The derived key.</returns>
        /// <remarks>
        /// The key consists of the first 24 bytes (192bits) of the SHA256-Hash of the UTF8-encoded password.
        /// </remarks>
        private static byte[] KeyFromPassword(string password)
        {
            using (var sha = new SHA256CryptoServiceProvider())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password)).Take(24).ToArray();
            }
        }
    }
}
