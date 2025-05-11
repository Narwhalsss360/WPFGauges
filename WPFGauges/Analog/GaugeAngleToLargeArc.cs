using System.Globalization;
using System.Windows.Data;

namespace WPFGauges.Analog
{
    [ValueConversion(typeof(double), typeof(bool))]
    class GaugeAngleToLargeArc : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double angle)
                throw new ArgumentException($"{typeof(GaugeAngleToLargeArc)} requires ${typeof(double)}");
            return angle >= 180;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
