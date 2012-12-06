using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace TfsConnector
{
    /// <summary>
    /// Base class for ViewModels.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Notifies that a property changed.
        /// </summary>
        /// <param name="property">The property that changed.</param>
        protected virtual void NotifyPropertyChanged(Expression<Func<object>> property)
        {
            MemberExpression memberExpression;
            var unaryExpression = property.Body as UnaryExpression;
            if (unaryExpression != null)
                memberExpression = unaryExpression.Operand as MemberExpression;
            else
                memberExpression = property.Body as MemberExpression;

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    NotifyPropertyChanged(propertyInfo.Name);
                    return;
                }
            }

            throw new ArgumentException(
                "The property is not a Expression with a MemberExpresion with type PropertyInfo", "property");
        }

        /// <summary>
        /// Notifies that a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the specified field.
        /// </summary>
        /// <typeparam name="TProp">The type of the prop.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="property">The property.</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        protected bool Set<TProp>(ref TProp field, TProp value, Expression<Func<object>> property)
        {
            if (!Equals(field, value))
            {
                field = value;
                NotifyPropertyChanged(property);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the specified set and get action for the property.
        /// </summary>
        /// <typeparam name="T">The type to be set.</typeparam>
        /// <param name="setAction">The set action.</param>
        /// <param name="getAction">The get action.</param>
        /// <param name="value">The value.</param>
        /// <param name="property">The property.</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        protected bool Set<T>(Action<T> setAction, Func<T> getAction, T value, Expression<Func<object>> property)
        {
            if (!Equals(getAction(), value))
            {
                setAction(value);
                NotifyPropertyChanged(property);
                return true;
            }

            return false;
        }
    }
}