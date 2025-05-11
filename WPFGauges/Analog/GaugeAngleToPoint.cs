using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFGauges.Analog
{
    [ValueConversion(typeof(double), typeof(double))]
    class GaugeAngleToPoint : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double angle = AnalogGauge.DEFAULT_ANGLE_SPAN;
            if (value is double angleValue)
                angle = angleValue;
            double gap = 360 - angle;

            string lowered = (parameter as string)?.Trim().ToLower() ?? "";
            angle *= Math.PI / 180;
            gap *= Math.PI / 180;

            double xOffset = AnalogGauge.GAUGE_CENTER * (Math.Sin(angle + gap / 2) + 1);
            double yOffset = AnalogGauge.GAUGE_CENTER * (Math.Cos(gap / 2) + 1);

            return new Point(lowered.StartsWith("right") ? AnalogGauge.GAUGE_SIZE - xOffset : xOffset, yOffset);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidProgramException();
        }
    }
}
