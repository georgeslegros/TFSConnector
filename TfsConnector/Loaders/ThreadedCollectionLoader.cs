using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace TfsConnector.Loaders
{
    public abstract class ThreadedCollectionLoader<T> : ICollectionLoader<T>
    {
        private void InternalLoad(object callback)
        {
            DisplayResults(LoadData(), callback as Action<IEnumerable<T>>);
        }

        protected abstract IEnumerable<T> LoadData();

        protected void DisplayResults(IEnumerable<T> collection, Action<IEnumerable<T>> callback)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() => callback(collection)));
        }

        protected virtual void PostBindingAction(Selector selector)
        {

        }


        public void Load(Action<IEnumerable<T>> callback)
        {
            ParameterizedThreadStart pts = InternalLoad;
            var thread = new Thread(pts);
            thread.Start(callback);
        }
    }
}