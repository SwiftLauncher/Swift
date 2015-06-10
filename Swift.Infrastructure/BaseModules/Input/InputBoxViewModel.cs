using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Swift.Extensibility;
using Swift.Extensibility.Events;
using Swift.Extensibility.Input;
using Swift.Extensibility.Input.Functions;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Internal.Events;
using Swift.Extensibility.Services;
using Swift.Extensibility.UI;

namespace Swift.Infrastructure.BaseModules.Input
{
    public class InputBoxViewModel : ViewModelBase, IInitializationAware
    {
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
                if (Equals(value, _currentInput)) return;
                Set(ref _currentInput, value);
                OnInputChanged(_currentInput);
            }
        }

        public DelegateCommand<KeyEventArgs> PreviewKeyDownCommand { get; private set; }

        [Import]
        private IPluginServices _pluginServices;
        [Import]
        private IFunctionManager _functionManager;

        private FunctionInfo _defaultFunction;

        public InputBoxViewModel()
        {
            PreviewKeyDownCommand = new DelegateCommand<KeyEventArgs>(PreviewKeyDown);
        }

        private void PreviewKeyDown(KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Down:
                    // TODO handle focus movement
                    break;
                case Key.Enter:
                    // request execution
                    var input = _pluginServices.GetService<IInputParser>().Parse(CurrentInput);
                    var exectype = Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down ? ExecutionType.HideBeforeExecution : ExecutionType.Default; // TODO use ExecutionType
                    _functionManager.Invoke(
                        _functionManager.HasMatchingFunction(input)
                            ? _functionManager.GetMatchingFunction(input)
                            : _defaultFunction, input);
                    break;
            }
        }

        private void OnInputChanged(string newInput)
        {
            var c = _pluginServices.GetService<IEventBroker>().GetChannel<InputChangedEventArgs>(Constants.EventNames.InputChanged);
            var i = _pluginServices.GetService<IInputParser>().Parse(newInput);
            c.Publish(new InputChangedEventArgs(i));
            if (_functionManager.HasMatchingFunction(i))
            {
                var f = _functionManager.GetMatchingFunction(i);
                if (f.CallMode == FunctionCallMode.Continuous)
                {
                    _functionManager.Invoke(f, i);
                }
            }
            else if (_defaultFunction.CallMode == FunctionCallMode.Continuous)
                _functionManager.Invoke(_defaultFunction, i);
        }

        public int InitializationPriority => 0;

        public void OnInitialization(InitializationEventArgs args)
        {
            _pluginServices.GetService<IUiService>().AddUiResource(new Uri("pack://application:,,,/Swift.Infrastructure;component/BaseModules/Input/InputBoxTemplates.xaml", UriKind.Absolute));
            _pluginServices.GetService<IUiService>().Navigate(this, ViewTargetsInternal.InputBoxPlaceHolder);
            _defaultFunction = _functionManager.GetFunctions().First(_ => _.FullName == "dataitems"); // TODO move to settings
        }
    }
}
