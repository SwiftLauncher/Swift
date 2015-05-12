using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Swift.Toolkit
{
    public class SyncedViewModelCollection<TModel, TViewModel> : IList<TViewModel>, INotifyCollectionChanged
    {
        private ThreadSafeObservableCollection<TViewModel> _viewModels;
        private IList<TModel> _models;
        private Func<TModel, TViewModel> _viewModelConstructor;
        private Func<TViewModel, TModel> _modelSelector;

        public SyncedViewModelCollection(IList<TModel> models, Func<TModel, TViewModel> viewModelConstructor, Func<TViewModel, TModel> modelSelector)
        {
            if (!(models is INotifyCollectionChanged))
                throw new ArgumentException("The model collection has to implement INotifyCollectionChanged.");

            _viewModels = new ThreadSafeObservableCollection<TViewModel>();
            _viewModels.CollectionChanged += (sender, e) =>
            {
                var c = CollectionChanged;
                if (c != null)
                {
                    c(sender, e);
                }
            };

            _viewModelConstructor = viewModelConstructor;
            _modelSelector = modelSelector;
            _models = models;

            if (models == null) return;

            foreach (var m in models)
            {
                _viewModels.Add(viewModelConstructor(m));
            }
            (_models as INotifyCollectionChanged).CollectionChanged += HandleModelCollectionChange;
        }

        private void HandleModelCollectionChange(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (TModel newItem in args.NewItems)
                {
                    _viewModels.Add(_viewModelConstructor(newItem));
                }
            }

            if (args.OldItems != null)
            {
                foreach (TModel oldItem in args.OldItems)
                {
                    var oldVM = _viewModels.First(_ => _modelSelector(_).Equals(oldItem));
                    _viewModels.Remove(oldVM);
                }
            }
        }

        #region IList<T> Implementation

        public int IndexOf(TViewModel item)
        {
            return _viewModels.IndexOf(item);
        }

        public void Insert(int index, TViewModel item)
        {
            _viewModels.Insert(index, item);
            if (_models == null) return;
            _models.Insert(index, _modelSelector(item));
        }

        public void RemoveAt(int index)
        {
            var vm = _viewModels[index];
            _viewModels.Remove(vm);
            if (_models == null) return;
            _models.Remove(_modelSelector(vm));
        }

        public TViewModel this[int index]
        {
            get
            {
                return _viewModels[index];
            }
            set
            {
                _viewModels[index] = value;
                if (_models == null) return;
                if (!_models.Contains(_modelSelector(value)))
                {
                    _models.Add(_modelSelector(value));
                }
            }
        }

        public void Add(TViewModel item)
        {
            _viewModels.Add(item);
            if (_models == null) return;
            _models.Add(_modelSelector(item));
        }

        public void Clear()
        {
            _viewModels.Clear();
            if (_models == null) return;
            _models.Clear();
        }

        public bool Contains(TViewModel item)
        {
            return _viewModels.Contains(item);
        }

        public void CopyTo(TViewModel[] array, int arrayIndex)
        {
            _viewModels.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _viewModels.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TViewModel item)
        {
            try
            {
                if (_viewModels.Remove(item))
                {
                    if (_models == null) return false;
                    return _models.Remove(_modelSelector(item));
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public IEnumerator<TViewModel> GetEnumerator()
        {
            return _viewModels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _viewModels.GetEnumerator();
        }

        #endregion

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
