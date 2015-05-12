using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Swift.Extensibility;
using Swift.Extensibility.Events;
using Swift.Extensibility.Input;
using Swift.Extensibility.Services;
using Swift.Extensibility.UI;
using Swift.Toolkit;

namespace Swift.AppServices.BaseModules
{
    [Export]
    public class DataViewViewModel : ViewModelBase, IPluginServiceUser, IInitializationAware
    {
        #region Properties & Commands

        private DataItem _selectedItem;
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public DataItem SelectedItem
        {
            get { return _selectedItem; }
            set { Set(ref _selectedItem, value); }
        }

        private ThreadSafeObservableCollection<DataItem> _dataItems = new ThreadSafeObservableCollection<DataItem>();
        /// <summary>
        /// Sets and gets the DataItems property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ThreadSafeObservableCollection<DataItem> DataItems
        {
            get
            {
                return _dataItems;
            }
            set
            {
                Set(ref _dataItems, value);
            }
        }

        public DelegateCommand<KeyEventArgs> ListKeyDownCommand
        {
            get
            {
                return new DelegateCommand<KeyEventArgs>(_ =>
                {
                    if (_.Key == Key.Up && DataItems != null && SelectedItem != null && DataItems.IndexOf(SelectedItem) == 0)
                    {
                        _eventAggregator.GetEvent<FocusChangeRequestedEvent>().Publish(new FocusChangeRequestedEventArgs(FocusTargets.AutoCompleteBoxSuggestionsTarget, FocusChangeType.End));
                    }
                    else if (_.Key == Key.Enter && SelectedItem != null && SelectedItem.ExecutionCallback != null)
                    {
                        // default behaviour: hide without shift
                        //var shiftDown = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
                        //_eventAggregator.GetEvent<ExecutionRequestedEvent>().Publish(new ExecutionRequestedEventArgs(shiftDown ? ExecutionType.Default : ExecutionType.HideBeforeExecution, null, SelectedItem.ExecutionCallback));
                        var exectype = Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down ? ExecutionType.HideBeforeExecution : ExecutionType.Default;
                        _pluginServices.GetService<IEventBroker>().GetChannel<ExecutionRequestedEventArgs>(InternalConstants.EventNames.ExecutionRequested).Publish(new ExecutionRequestedEventArgs(exectype, null, SelectedItem.ExecutionCallback));
                    }
                });
            }
        }

        #endregion

        [Import]
        private IEventAggregator _eventAggregator;
        [Import]
        private IDataItemHandler _dataItemHandler;

        private IPluginServices _pluginServices;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public DataViewViewModel()
        {
        }

        /// <summary>
        /// Handles the <see cref="E:InputChanged" /> event.
        /// </summary>
        /// <param name="obj">The <see cref="InputChangedEventArgs"/> instance containing the event data.</param>
        private async void OnInputChanged(InputChangedEventArgs args)
        {
            IList<DataItem> items = DataItems;
            DataItem selectedItem = SelectedItem;
            await _dataItemHandler.GetMatchingItemsAsync(args.NewInput, ref items, new CancellationToken());
            if (DataItems.Count > 0)
            {
                SelectedItem = DataItems[0];
                _pluginServices.GetService<IUIService>().Navigate(this, ViewTargetsInternal.CenterView);
            }
        }

        private void OnExecutionRequested(ExecutionRequestedEventArgs args)
        {
            if (args.ExecutionCallback != null)
            {
                args.ExecutionCallback();
            }
            else
            {

            }
        }

        #region IPluginServiceUser Implementation

        /// <summary>
        /// Provides an implementation of plugin services.
        /// </summary>
        /// <param name="pluginServices">The plugin services.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SetPluginServices(IPluginServices pluginServices)
        {
            _pluginServices = pluginServices;
        }

        #endregion

        #region IInitializationAware Implementation

        public int InitializationPriority
        {
            get { return 0; }
        }

        public void OnInitialization(InitializationEventArgs args)
        {
            _pluginServices.GetService<IUIService>().AddUIResource(new Uri("pack://application:,,,/Swift.AppServices;component/BaseModules/Data/DataViewTemplates.xaml", UriKind.Absolute));
            _pluginServices.GetService<IEventBroker>().GetChannel<InputChangedEventArgs>(Constants.EventNames.InputChanged).Subscribe(OnInputChanged);
            _pluginServices.GetService<IEventBroker>().GetChannel<ExecutionRequestedEventArgs>(InternalConstants.EventNames.ExecutionRequested).Subscribe(OnExecutionRequested);
        }

        #endregion
    }
}
