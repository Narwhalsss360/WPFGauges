using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFGauges.Data;

namespace WPFGauges.Bubble
{
    /// <summary>
    /// Interaction logic for LevelBubbleGauge.xaml
    /// </summary>
    public partial class LevelBubbleGauge : UserControl
    {
        public delegate DoubleAnimation BubbleAnimationGenerator(double from, double to);

        public static readonly double GAUGE_WIDTH = 160;

        public static readonly double GAUGE_HEIGHT = 40;

        public static readonly double GAUGE_HORIZONTAL_CENTER = GAUGE_WIDTH / 2;

        public static readonly double GAUGE_VERTICAL_CENTER = GAUGE_HEIGHT / 2;

        public static readonly double BUBBLE_RADIUS = GAUGE_HEIGHT / 2;

        public static readonly double BUBBLE_DIAMETER = BUBBLE_RADIUS * 2;

        public static readonly Brush DEFAULT_BUBBLE_GAUGE_BACKGROUND = Brushes.Green;

        public static readonly double DEFAULT_VALUE = 0.Clamp(-1, 1);

        public static readonly double DEFAULT_BUBBLE_TRANSLATE = LinearMap.Map(-1, 1, -(GAUGE_HORIZONTAL_CENTER - BUBBLE_RADIUS), GAUGE_HORIZONTAL_CENTER - BUBBLE_RADIUS, DEFAULT_VALUE);

        public static readonly BubbleAnimationGenerator DEFAULT_ANIMATOR = (from, to) => new(from, to, TimeSpan.Zero);

        public static readonly DependencyProperty BubbleGaugeBackgroundProperty = DP.RegisterProperty<LevelBubbleGauge>(nameof(BubbleGaugeBackground), DEFAULT_BUBBLE_GAUGE_BACKGROUND);

        public static readonly DependencyProperty ValueProperty = DP.RegisterProperty<LevelBubbleGauge>(nameof(Value), DEFAULT_VALUE);

        public static readonly DependencyProperty BubbleTranslateProperty = DP.RegisterProperty<LevelBubbleGauge>(nameof(BubbleTranslate), DEFAULT_BUBBLE_TRANSLATE);

        public static readonly DependencyProperty AnimatorProperty = DP.RegisterProperty<LevelBubbleGauge>(nameof(Animator), DEFAULT_ANIMATOR);

        readonly LinearMap _translateMap = new(new(-1, 1), new(-(GAUGE_HORIZONTAL_CENTER - BUBBLE_RADIUS), GAUGE_HORIZONTAL_CENTER - BUBBLE_RADIUS));

        public Brush BubbleGaugeBackground
        {
            get => (Brush)GetValue(BubbleGaugeBackgroundProperty);
            set => SetValue(BubbleGaugeBackgroundProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, ApplyBubble(value.Clamp(-1, 1)));
        }

        public double BubbleTranslate
        {
            get => (double)GetValue(BubbleTranslateProperty);
            protected set => SetValue(BubbleTranslateProperty, value);
        }

        public BubbleAnimationGenerator Animator
        {
            get => (BubbleAnimationGenerator)GetValue(AnimatorProperty);
            set => SetValue(AnimatorProperty, value);
        }

        public LevelBubbleGauge()
        {
            DataContext = this;
            InitializeComponent();
        }

        protected double ApplyBubble(double value)
        {
            BeginAnimation(
                BubbleTranslateProperty,
                Animator(
                    _translateMap.Map(Value),
                    _translateMap.Map(value)
                ),
                HandoffBehavior.SnapshotAndReplace
            );
            return value;
        }
    }
}
