using System.Reflection;
using System.Windows;

namespace WPFGauges
{
    public static class DP
    {
        public static DependencyProperty RegisterProperty<T>(string name) where T : DependencyObject
        {
            if (typeof(T).GetProperty(name) is not PropertyInfo property)
                throw new ArgumentException($"{nameof(RegisterProperty)}<{typeof(T)}>({nameof(name)}={name}): Property not found", nameof(name));
            return DependencyProperty.Register(name, property.PropertyType, typeof(T));
        }

        public static DependencyProperty RegisterProperty<T>(string name, object defaultValue)
        {
            if (typeof(T).GetProperty(name) is not PropertyInfo property)
                throw new ArgumentException($"{nameof(RegisterProperty)}<{typeof(T)}>({nameof(name)}={name}, {nameof(defaultValue)}={defaultValue}): Property not found", nameof(name));
            return DependencyProperty.Register(name, property.PropertyType, typeof(T), new PropertyMetadata(defaultValue));
        }
    }
}
