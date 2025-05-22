using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FyreWorksPM.Utilities;

public static class ViewModelHookHelper
{
    public static void AttachCollectionHandlers<T>(
        ObservableCollection<T> collection,
        PropertyChangedEventHandler handler,
        Action updateTotals)
        where T : INotifyPropertyChanged
    {
        collection.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
            {
                foreach (T item in e.NewItems)
                    item.PropertyChanged += handler;
            }

            if (e.OldItems != null)
            {
                foreach (T item in e.OldItems)
                    item.PropertyChanged -= handler;
            }

            updateTotals();
        };

        // Attach to existing items if any
        foreach (T item in collection)
            item.PropertyChanged += handler;
    }
}
