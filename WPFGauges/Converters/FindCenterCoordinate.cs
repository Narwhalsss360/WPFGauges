using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace WPFGauges.Converters
{
    [ValueConversion(typeof(FrameworkElement), typeof(double))]
    public class FindCenterCoordinate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not FrameworkElement element)
                throw new ArgumentException($"Can use {typeof(FindCenterCoordinate)} to convert {typeof(FrameworkElement)} to {typeof(double)}", nameof(value));

            string prepared = (parameter as string)?.Trim().ToLower() ?? "";
            bool actual = prepared.StartsWith("actual");
            bool y = prepared.EndsWith("y");


            return (
                y ?
                (actual ? element.ActualHeight : element.Height) :
                (actual ? element.ActualWidth : element.Width)
            ) / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot ConvertBack() from double");
        }
    }
}
