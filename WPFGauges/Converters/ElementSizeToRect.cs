using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace WPFGauges.Converters
{
    [ValueConversion(typeof(FrameworkElement), typeof(Rect))]
    public class ElementSizeToRect : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not FrameworkElement element)
                throw new ArgumentException($"Can only use {typeof(ElementSizeToRect)} to convert {typeof(FrameworkElement)} to {typeof(Rect)}", nameof(value));

            string prepared = (parameter as string)?.Trim().ToLower() ?? "";
            bool actual = prepared.StartsWith("actual");

            return new Rect(
                0,
                0,
                actual ? element.ActualWidth : element.Width,
                actual ? element.ActualHeight : element.Height
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot ConvertBack() from Rect");
        }
    }
}
