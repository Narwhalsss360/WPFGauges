using System.Globalization;
using System.Windows.Data;

namespace WPFGauges.Converters
{
    public class Scale : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double valueDouble)
                throw new ArgumentException($"value must be {typeof(double)}.", nameof(value));
            double scale = double.Parse((parameter as string)?.Trim() ?? "1");
            return valueDouble * scale;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot ConvertBack() from double");
        }
    }
}
