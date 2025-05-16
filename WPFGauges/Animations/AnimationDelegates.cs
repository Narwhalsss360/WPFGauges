using System.Windows.Media.Animation;

namespace WPFGauges.Animations
{
    public delegate DoubleAnimation DoubleAnimationGenerator(double from, double to);

    public static class Generators
    {
        public static class DoubleAnimation
        {
            public static readonly DoubleAnimationGenerator None = (from, to) => new(from, to, TimeSpan.Zero);
        }
    }
}
