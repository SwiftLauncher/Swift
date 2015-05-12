using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Swift.Extensibility;
using Swift.Extensibility.Input;
using Swift.Extensibility.Services;

namespace Swift.Infrastructure.BaseModules.Hotkeys
{
    [Export(typeof(IHotkeyService))]
    public class SwiftHotkeyManager : IHotkeyService, IInitializationAware, IShutdownAware, IPluginServiceUser
    {
        private IPluginServices _pluginServices;
        private Dictionary<string, HotkeyToken> _hotkeys = new Dictionary<string, HotkeyToken>();

        #region Initialization and Shutdown

        /// <summary>
        /// Gets the initialization priority. Higher values lead to later initialization.
        /// </summary>
        /// <value>
        /// The initialization priority.
        /// </value>
        public int InitializationPriority
        {
            get { return 0; }
        }

        /// <summary>
        /// Handles the <see cref="E:Initialization" /> event.
        /// </summary>
        /// <param name="args">The <see cref="T:Swift.Extensibility.Services.InitializationEventArgs" /> instance containing the event data.</param>
        public void OnInitialization(InitializationEventArgs args)
        {
            //RegisterHotkeys();
        }

        /// <summary>
        /// Gets the shutdown priority. Higher values lead to later shutdown.
        /// </summary>
        /// <value>
        /// The shutdown priority.
        /// </value>
        public int ShutdownPriority
        {
            get { return 0; }
        }

        /// <summary>
        /// Handles the <see cref="E:Shutdown" /> event.
        /// </summary>
        /// <param name="args">The <see cref="T:Swift.Extensibility.Services.ShutdownEventArgs" /> instance containing the event data.</param>
        public void OnShutdown(ShutdownEventArgs args)
        {
            // Unregister all system-wide hotkeys
            foreach (var hk in _hotkeys.Values.Where(_ => _.Binding == null))
                NHotkey.Wpf.HotkeyManager.Current.Remove(hk.Name);
        }

        #endregion

        public void SetPluginServices(IPluginServices pluginServices)
        {
            _pluginServices = pluginServices;
        }

        #region IHotkeyService Implementation

        /// <summary>
        /// Determines whether a hotkey with the specified key and modifiers is registered.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>
        /// True, if a hotkey with the given key and modifiers is registered. False otherwise.
        /// </returns>
        public bool IsRegistered(Key key, ModifierKeys modifiers)
        {
            return _hotkeys.Values.Any(_ => _.Key == key && _.Modifiers == modifiers);
        }

        /// <summary>
        /// Determines whether a hotkey the specified name is registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// True, if a hotkey with the given name is registered. False otherwise.
        /// </returns>
        public bool IsRegistered(string name)
        {
            return _hotkeys.ContainsKey(name);
        }

        /// <summary>
        /// Occurs whenever a public hotkey is pressed.
        /// </summary>
        public event HotkeyEventHandler OnHotkey;

        /// <summary>
        /// Registers the hotkey. Only possible if no other hotkey with the same name or hotkey combination is already registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="visibility">The visibility.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IDIsposable that will unregister the hotkey when disposed.
        /// </returns>
        public IDisposable RegisterHotkey(string name, Key key, ModifierKeys modifiers, HotkeyMode mode, HotkeyVisibility visibility, Action<HotkeyEventArgs> callback)
        {
            if (IsRegistered(name) || IsRegistered(key, modifiers))
            {
                throw new NotSupportedException("Multiple hotkeys with the same name of keys are not supported!");
            }
            var token = new HotkeyToken(name, key, modifiers, visibility, callback);
            if (mode == HotkeyMode.Application)
            {
                token.Binding = new InputBinding(new SimpleCallbackCommand(HandleHotkey, new HotkeyEventArgs(name)), new KeyGesture(key, modifiers));
                System.Windows.Application.Current.MainWindow.InputBindings.Add(token.Binding);
                token.OnDispose += _ =>
                {
                    System.Windows.Application.Current.MainWindow.InputBindings.Remove(_.Binding);
                };
            }
            else
            {
                NHotkey.Wpf.HotkeyManager.Current.AddOrReplace(name, key, modifiers, (s, _) => HandleHotkey(new Swift.Extensibility.Input.HotkeyEventArgs(_.Name)));
                token.OnDispose += _ =>
                {
                    NHotkey.Wpf.HotkeyManager.Current.Remove(_.Name);
                };
            }
            _hotkeys.Add(name, token);
            return token;
        }

        /// <summary>
        /// Handles the hotkey.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="NHotkey.HotkeyEventArgs"/> instance containing the event data.</param>
        private void HandleHotkey(HotkeyEventArgs args)
        {
            var token = _hotkeys[args.Name];
            if (token.Callback != null)
                token.Callback(new HotkeyEventArgs(args.Name));
            if (token.Visibility == HotkeyVisibility.Public)
            {
                if (OnHotkey != null)
                    OnHotkey(new HotkeyEventArgs(args.Name));
            }
        }

        private class SimpleCallbackCommand : ICommand
        {
            private Action<HotkeyEventArgs> Callback { get; set; }
            private HotkeyEventArgs Args { get; set; }

            public SimpleCallbackCommand(Action<HotkeyEventArgs> callback, HotkeyEventArgs args)
            {
                Callback = callback;
                Args = args;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                if (Callback != null)
                    Callback(Args);
            }
        }

        #endregion
    }

    /// <summary>
    /// Class for saving the hotkey information and unregistering them.
    /// </summary>
    public class HotkeyToken : IDisposable
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Key Key { get; private set; }

        /// <summary>
        /// Gets the modifiers.
        /// </summary>
        /// <value>
        /// The modifiers.
        /// </value>
        public ModifierKeys Modifiers { get; private set; }

        /// <summary>
        /// Gets the visibility.
        /// </summary>
        /// <value>
        /// The visibility.
        /// </value>
        public HotkeyVisibility Visibility { get; private set; }

        /// <summary>
        /// Gets the callback.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        public Action<HotkeyEventArgs> Callback { get; private set; }

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>
        /// The binding.
        /// </value>
        public InputBinding Binding { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (OnDispose != null)
                OnDispose(this);
        }

        /// <summary>
        /// Occurs when disposed.
        /// </summary>
        public event Action<HotkeyToken> OnDispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyToken" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="visibility">The visibility.</param>
        public HotkeyToken(string name, Key key, ModifierKeys modifiers, HotkeyVisibility visibility, Action<HotkeyEventArgs> callback)
        {
            Name = name;
            Key = key;
            Modifiers = modifiers;
            Visibility = visibility;
            Callback = callback;
        }
    }

}
