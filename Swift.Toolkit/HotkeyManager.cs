using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace Swift.Toolkit
{
    public static class HotkeyManager
    {
        private static Dictionary<string, HotKey> _hotkeys = new Dictionary<string, HotKey>();

        /// <summary>
        /// Registers a system-wide hotkey with the given ID.
        /// </summary>
        /// <param name="id">The unique ID for this hotkey.</param>
        /// <param name="k">The key.</param>
        /// <param name="m">Modifier keys.</param>
        /// <param name="callback">The callback action that will be executed when the hotkey is pressed.</param>
        public static void RegisterHotkey(string id, Key k, ModifierKeys m, Action<string> callback)
        {
            if (IsHotkeyRegistered(id))
                throw new InvalidOperationException("A HotKey with this id is already registered.");
            var h = new HotKey(id, k, m.ToKeyModifier(), _ => callback(_.UserId), true);
            _hotkeys.Add(id, h);
        }

        /// <summary>
        /// Unregisters the hotkey with the given ID.
        /// </summary>
        /// <param name="id">The ID of the HotKey to be unregistered.</param>
        public static void UnregisterHotkey(string id)
        {
            if (!IsHotkeyRegistered(id))
                return;
            var h = _hotkeys[id];
            _hotkeys.Remove(id);
            h.Dispose();
        }

        /// <summary>
        /// Checks whether a hotkey with the given ID is registered.
        /// </summary>
        /// <param name="id">The ID to be checked.</param>
        /// <returns>True, if a hotkey with this ID has already been registered, false otherwise.</returns>
        public static bool IsHotkeyRegistered(string id)
        {
            return _hotkeys.ContainsKey(id);
        }

        /// <summary>
        /// Unregisters all hotkeys. Call this method on application shutdown.
        /// </summary>
        public static void Shutdown()
        {
            foreach (var h in _hotkeys.Values)
            {
                h.Dispose();
            }
        }

        private static KeyModifier ToKeyModifier(this ModifierKeys k)
        {
            var mod = KeyModifier.None;
            if ((k & ModifierKeys.Alt) == ModifierKeys.Alt)
                mod |= KeyModifier.Alt;
            if ((k & ModifierKeys.Control) == ModifierKeys.Control)
                mod |= KeyModifier.Ctrl;
            if ((k & ModifierKeys.Shift) == ModifierKeys.Shift)
                mod |= KeyModifier.Shift;
            if ((k & ModifierKeys.Windows) == ModifierKeys.Windows)
                mod |= KeyModifier.Win;
            return mod;
        }
    }

    internal class HotKey : IDisposable
    {
        private static Dictionary<int, HotKey> _dictHotKeyToCalBackProc;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        internal const int WmHotKey = 0x0312;

        private bool _disposed = false;

        internal Key Key { get; private set; }
        internal KeyModifier KeyModifiers { get; private set; }
        internal Action<HotKey> Action { get; private set; }
        internal int Id { get; set; }
        internal string UserId { get; set; }

        // ******************************************************************
        internal HotKey(string id, Key k, KeyModifier keyModifiers, Action<HotKey> action, bool register = true)
        {
            Key = k;
            KeyModifiers = keyModifiers;
            Action = action;
            UserId = id;
            if (register)
            {
                Register();
            }
        }

        // ******************************************************************
        internal bool Register()
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(Key);
            Id = virtualKeyCode + ((int)KeyModifiers * 0x10000);
            bool result = RegisterHotKey(IntPtr.Zero, Id, (UInt32)KeyModifiers, (UInt32)virtualKeyCode);

            if (_dictHotKeyToCalBackProc == null)
            {
                _dictHotKeyToCalBackProc = new Dictionary<int, HotKey>();
                ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
            }

            _dictHotKeyToCalBackProc.Add(Id, this);
            return result;
        }

        // ******************************************************************
        internal void Unregister()
        {
            HotKey hotKey;
            if (_dictHotKeyToCalBackProc.TryGetValue(Id, out hotKey))
            {
                UnregisterHotKey(IntPtr.Zero, Id);
            }
        }

        // ******************************************************************
        private static void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WmHotKey)
                {
                    HotKey hotKey;

                    if (_dictHotKeyToCalBackProc.TryGetValue((int)msg.wParam, out hotKey))
                    {
                        if (hotKey.Action != null)
                        {
                            hotKey.Action.Invoke(hotKey);
                        }
                        handled = true;
                    }
                }
            }
        }

        // ******************************************************************
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // ******************************************************************
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be _disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be _disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    Unregister();
                }

                // Note disposing has been done.
                _disposed = true;
            }
        }
    }

    // ******************************************************************
    [Flags]
    internal enum KeyModifier
    {
        None = 0x0000,
        Alt = 0x0001,
        Ctrl = 0x0002,
        NoRepeat = 0x4000,
        Shift = 0x0004,
        Win = 0x0008
    }

}
