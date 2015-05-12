using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Swift.Extensibility;
using Swift.Extensibility.Events;
using Swift.Extensibility.Input;
using Swift.Extensibility.Services;
using Swift.Extensibility.UI;

namespace Swift.AppServices.BaseModules
{
    public class InputBoxViewModel : ViewModelBase, IInitializationAware, IPluginServiceUser
    {
        #region Properties

        /// <summary>
        /// Backing field for <see cref="CurrentInput"/>.
        /// </summary>
        private string _currentInput;
        /// <summary>
        /// Gets or sets the current input.
        /// </summary>
        /// <value>
        /// The current input.
        /// </value>
        public string CurrentInput
        {
            get { return _currentInput; }
            set
            {
                if (!Object.Equals(value, _currentInput))
                {
                    Set(ref _currentInput, value);
                    var c = _pluginServices.GetService<IEventBroker>().GetChannel<InputChangedEventArgs>(Constants.EventNames.InputChanged);
                    var i = _pluginServices.GetService<IInputParser>().Parse(_currentInput);
                    c.Publish(new InputChangedEventArgs(i));
                }
            }
        }

        #endregion

        #region Commands

        public DelegateCommand<KeyEventArgs> PreviewKeyDownCommand { get; private set; }

        #endregion

        private IPluginServices _pluginServices;

        public InputBoxViewModel()
        {
            PreviewKeyDownCommand = new DelegateCommand<KeyEventArgs>(PreviewKeyDown);
        }

        private void PreviewKeyDown(KeyEventArgs args)
        {
            if (args.Key == Key.Down)
            {
                // TODO handle focus movement

            }
            else if (args.Key == Key.Enter)
            {
                // request execution
                var input = _pluginServices.GetService<IInputParser>().Parse(CurrentInput);
                var exectype = Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down ? ExecutionType.HideBeforeExecution : ExecutionType.Default;
                _pluginServices.GetService<IEventBroker>().GetChannel<ExecutionRequestedEventArgs>(InternalConstants.EventNames.ExecutionRequested).Publish(new ExecutionRequestedEventArgs(exectype, input));
            }
        }

        #region IInitializationAware Implementation

        public int InitializationPriority { get { return 0; } }

        public void OnInitialization(InitializationEventArgs args)
        {
            _pluginServices.GetService<IUIService>().AddUIResource(new Uri("pack://application:,,,/Swift.AppServices;component/BaseModules/Input/InputBoxTemplates.xaml", UriKind.Absolute));
            _pluginServices.GetService<IUIService>().Navigate(this, ViewTargetsInternal.InputBoxPlaceHolder);
        }

        #endregion

        #region IPluginServiceUser Implementation

        public void SetPluginServices(IPluginServices pluginServices)
        {
            _pluginServices = pluginServices;
        }

        #endregion
    }
}
