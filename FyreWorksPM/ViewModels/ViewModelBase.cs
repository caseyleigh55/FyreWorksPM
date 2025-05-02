

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FyreWorksPM.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels.
    /// Uses a backing dictionary for dynamic property storage.
    /// Implements INotifyPropertyChanged to update UI bindings automatically.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        // Backing store for all property values
        private readonly Dictionary<string, object?> _values = new();

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets a property's value from the backing dictionary.
        /// </summary>
        /// <typeparam name="T">Expected property type.</typeparam>
        /// <param name="propertyName">Auto-injected name of the property.</param>
        /// <returns>Stored value or default(T) if not found.</returns>
        protected T Get<T>([CallerMemberName] string propertyName = "")
        {
            if (_values.TryGetValue(propertyName, out var val) && val is T typed)
                return typed;

            return default!;
        }

        /// <summary>
        /// Sets a property's value and notifies the UI if it changed.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="value">New value to assign.</param>
        /// <param name="propertyName">Auto-injected name of the property.</param>
        /// <returns>True if the value was changed, false if it was the same.</returns>
        protected bool Set<T>(T value, [CallerMemberName] string propertyName = "")
        {
            if (_values.TryGetValue(propertyName, out var existing) &&
                EqualityComparer<T>.Default.Equals((T?)existing!, value))
                return false;

            _values[propertyName] = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Triggers the PropertyChanged event for a property.
        /// </summary>
        /// <param name="propertyName">Auto-injected name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
