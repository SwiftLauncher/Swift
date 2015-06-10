using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Swift.Extensibility;
using Swift.Extensibility.Events;
using Swift.Extensibility.Input;
using Swift.Extensibility.Input.Functions;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Internal.Events;
using Swift.Extensibility.Services;
using Swift.Extensibility.UI;
using Swift.Toolkit;

namespace Swift.Infrastructure.BaseModules.Data
{
    [Export]
    public class DataViewViewModel : ViewModelBase, IInitializationAware, ISwiftFunctionSource
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
                    else if (_.Key == Key.Enter && SelectedItem?.ExecutionCallback != null)
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
        [Import]
        private IPluginServices _pluginServices;

        [SwiftFunction("dataitems", CallMode = FunctionCallMode.Continuous)]
        [ParameterDescription("query", "The text to search for.")]
        public async void DataItemsMainFunction(string query)
        {
            IList<DataItem> items = DataItems;
            await _dataItemHandler.GetMatchingItemsAsync(query, ref items, new CancellationToken());
            if (DataItems.Count <= 0) return;
            SelectedItem = DataItems[0];
            _pluginServices.GetService<IUiService>().Navigate(this, ViewTargetsInternal.CenterView);
        }

        #region IInitializationAware Implementation

        public int InitializationPriority => 0;

        public void OnInitialization(InitializationEventArgs args)
        {
            _pluginServices.GetService<IUiService>().AddUiResource(new Uri("pack://application:,,,/Swift.Infrastructure;component/BaseModules/Data/DataViewTemplates.xaml", UriKind.Absolute));
        }

        #endregion
    }
}
