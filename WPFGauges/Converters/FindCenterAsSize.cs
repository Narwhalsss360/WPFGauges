using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFGauges.Converters
{
    [ValueConversion(typeof(FrameworkElement), typeof(Size))]
    class FindCenterAsSize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not FrameworkElement element)
                throw new ArgumentException($"Can use {typeof(FindCenterAsSize)} to convert {typeof(FrameworkElement)} to {typeof(Size)}", nameof(value));

            string prepared = (parameter as string)?.Trim().ToLower() ?? "";
            bool actual = prepared.StartsWith("actual");

            return new Size(
                (actual ? element.ActualWidth : element.Width) / 2,
                (actual ? element.ActualHeight : element.Height) / 2
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot ConvertBack() from Size");
        }
    }
}
