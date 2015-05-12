using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;

namespace Swift.Toolkit
{
    public class ThreadSafeObservableCollection<T> : IList<T>, INotifyCollectionChanged
    {
        private IList<T> collection = new List<T>();
        private Dispatcher _dispatcher;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private ReaderWriterLock _lock = new ReaderWriterLock();
        private Action<Action> dispatchAction;

        public ThreadSafeObservableCollection(bool asyncDispatch = false)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            if (asyncDispatch)
            {
                dispatchAction = _ => _dispatcher.BeginInvoke(_);
            }
            else
            {
                dispatchAction = _dispatcher.Invoke;
            }
        }

        public void Add(T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoAdd(item);
            else
                dispatchAction(() => DoAdd(item));
        }

        private void DoAdd(T item)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            collection.Add(item);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, collection.Count - 1));
            _lock.ReleaseWriterLock();
        }

        public void Clear()
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoClear();
            else
                dispatchAction(() => DoClear());
        }

        private void DoClear()
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            collection.Clear();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            _lock.ReleaseWriterLock();
        }

        public bool Contains(T item)
        {
            _lock.AcquireReaderLock(Timeout.Infinite);
            var result = collection.Contains(item);
            _lock.ReleaseReaderLock();
            return result;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            collection.CopyTo(array, arrayIndex);
            _lock.ReleaseWriterLock();
        }

        public int Count
        {
            get
            {
                _lock.AcquireReaderLock(Timeout.Infinite);
                var result = collection.Count;
                _lock.ReleaseReaderLock();
                return result;
            }
        }

        public bool IsReadOnly
        {
            get { return collection.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                return DoRemove(item);
            else
            {
                try
                {
                    dispatchAction(() => DoRemove(item));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private bool DoRemove(T item)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            var index = collection.IndexOf(item);
            if (index == -1)
            {
                _lock.ReleaseWriterLock();
                return false;
            }
            var result = collection.Remove(item);
            if (result && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            _lock.ReleaseWriterLock();
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            _lock.AcquireReaderLock(Timeout.Infinite);
            var result = collection.IndexOf(item);
            _lock.ReleaseReaderLock();
            return result;
        }

        public void Insert(int index, T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoInsert(index, item);
            else
                dispatchAction(() => DoInsert(index, item));
        }

        private void DoInsert(int index, T item)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            collection.Insert(index, item);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            _lock.ReleaseWriterLock();
        }

        public void RemoveAt(int index)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoRemoveAt(index);
            else
                dispatchAction(() => DoRemoveAt(index));
        }

        private void DoRemoveAt(int index)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            if (collection.Count == 0 || collection.Count <= index)
            {
                _lock.ReleaseWriterLock();
                return;
            }
            collection.RemoveAt(index);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            _lock.ReleaseWriterLock();

        }

        public T this[int index]
        {
            get
            {
                _lock.AcquireReaderLock(Timeout.Infinite);
                var result = collection[index];
                _lock.ReleaseReaderLock();
                return result;
            }
            set
            {
                _lock.AcquireWriterLock(Timeout.Infinite);
                if (collection.Count == 0 || collection.Count <= index)
                {
                    _lock.ReleaseWriterLock();
                    return;
                }
                collection[index] = value;
                _lock.ReleaseWriterLock();
            }

        }
    }

}
