using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace WPFGauges.Converters
{
    [ValueConversion(typeof(FrameworkElement), typeof(Point))]
    public class FindCenterPoint : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not FrameworkElement element)
                throw new ArgumentException($"Can use {typeof(FindCenterPoint)} to convert {typeof(FrameworkElement)} to {typeof(Point)}", nameof(value));

            string prepared = (parameter as string)?.Trim().ToLower() ?? "";
            bool actual = prepared.StartsWith("actual");

            return new Point(
                (actual ? element.ActualWidth : element.Width) / 2,
                (actual ? element.ActualHeight : element.Height) / 2
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot ConvertBack() from Point");
        }
    }
}
