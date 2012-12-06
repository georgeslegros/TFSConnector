using System;
using System.Collections.Generic;

namespace TfsConnector.Loaders
{
    public interface ICollectionLoader<T>
    {
        void Load(Action<IEnumerable<T>> callback);
    }
}