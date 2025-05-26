using System.Windows;
using System.Windows.Media.Animation;

namespace WPFGauges.Animations
{
    public delegate DoubleAnimation DoubleAnimationGenerator(double from, double to);

    public delegate ThicknessAnimation ThicknessAnimationGenerator(Thickness from, Thickness to);

    public static class Generators
    {
        public static class DoubleAnimation
        {
            public static readonly DoubleAnimationGenerator None = (from, to) => new(from, to, TimeSpan.Zero);
        }

        public static class ThicknessAnimation
        {
            public static readonly ThicknessAnimationGenerator None = (from, to) => new(from, to, TimeSpan.Zero);
        }
    }
}
