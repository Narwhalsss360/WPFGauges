using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFGauges.Animations;
using WPFGauges.Data;

namespace WPFGauges.Analog
{
    /// <summary>
    /// Interaction logic for AnalogGauge.xaml
    /// </summary>
    public partial class AnalogGauge : UserControl
    {
        public static readonly double GAUGE_SIZE = 100;

        public static readonly double GAUGE_CENTER = 50;

        public static readonly double DEFAULT_MINIMUM = 0;

        public static readonly double DEFAULT_MAXIMUM = 100;

        public static readonly double DEFAULT_ANGLE_SPAN = 270;

        public static readonly double DEFAULT_VALUE = (DEFAULT_MINIMUM + DEFAULT_MAXIMUM) / 2;

        public static readonly Func<Control> DEFAULT_NEEDLE_CONTROL_GENERATOR = () => new DefaultNeedle();

        public static readonly double DEFAULT_ANGLE_GAP = 360 - DEFAULT_ANGLE_SPAN;

        public static readonly double DEFAULT_HORIZONTAL_ANGLE_OFFSET = DEFAULT_ANGLE_GAP / 2;

        public static readonly double DEFAULT_MINIMUM_ANGLE = DEFAULT_HORIZONTAL_ANGLE_OFFSET - 90;

        public static readonly double DEFAULT_MAXIMUM_ANGLE = 270 - DEFAULT_HORIZONTAL_ANGLE_OFFSET;

        public static readonly double DEFAULT_NEEDLE_ANGLE = new LinearMap(new(DEFAULT_MINIMUM, DEFAULT_MAXIMUM), new(DEFAULT_MINIMUM_ANGLE, DEFAULT_MAXIMUM_ANGLE)).Map(DEFAULT_VALUE);

        public static readonly DoubleAnimationGenerator DEFAULT_ANIMATOR = (from, to) => new(from, to, TimeSpan.Zero);

        public static readonly double DEFAULT_OUTLINE_THICKNESS = 1;

        public static readonly Brush DEFAULT_OUTLINE_BRUSH = Brushes.Black;

        public static readonly double DEFAULT_PITCH = 0;

        public static readonly double DEFAULT_GRADUATION_THICKNESS = 1;

        public static readonly double DEFAULT_GRADUATION_LENGTH= 5;

        public static readonly Brush DEFAULT_GRADUATION_BRUSH = Brushes.Black;

        public static readonly int DEFAULT_LABEL_INTERVAL = 0;

        public static readonly int DEFAULT_LABEL_INTERVAL_OFFSET = 0;

        public static readonly LabelLevel DEFAULT_LABEL_LEVELING = LabelLevel.Leveled;

        public static readonly Func<Style> DEFAULT_LABEL_STYLE_GENERATOR = () => new()
        {
            Setters =
            {
                new Setter(TextBlock.FontSizeProperty, 4d),
                new Setter(VerticalAlignmentProperty, VerticalAlignment.Center),
                new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center)
            }
        };

        public static readonly DependencyProperty MinimumProperty = DP.RegisterProperty<AnalogGauge>(nameof(Minimum), DEFAULT_MINIMUM);

        public static readonly DependencyProperty MaximumProperty = DP.RegisterProperty<AnalogGauge>(nameof(Maximum), DEFAULT_MAXIMUM);

        public static readonly DependencyProperty AngleSpanProperty = DP.RegisterProperty<AnalogGauge>(nameof(AngleSpan), DEFAULT_ANGLE_SPAN);

        public static readonly DependencyProperty ValueProperty = DP.RegisterProperty<AnalogGauge>(nameof(Value), DEFAULT_VALUE);

        public static readonly DependencyProperty NeedleControlElementProperty = DP.RegisterProperty<AnalogGauge>(nameof(NeedleControlElement));

        public static readonly DependencyProperty NeedleAngleProperty = DP.RegisterProperty<AnalogGauge>(nameof(NeedleAngle), DEFAULT_NEEDLE_ANGLE);

        public static readonly DependencyProperty AnimatorProperty = DP.RegisterProperty<AnalogGauge>(nameof(Animator), DEFAULT_ANIMATOR);

        public static readonly DependencyProperty OutlineThicknessProperty = DP.RegisterProperty<AnalogGauge>(nameof(OutlineThickness), DEFAULT_OUTLINE_THICKNESS);

        public static readonly DependencyProperty OutlineBrushProperty = DP.RegisterProperty<AnalogGauge>(nameof(OutlineBrush), DEFAULT_OUTLINE_BRUSH);

        public static readonly DependencyProperty PitchProperty = DP.RegisterProperty<AnalogGauge>(nameof(Pitch), DEFAULT_PITCH);

        public static readonly DependencyProperty GraduationThicknessProperty = DP.RegisterProperty<AnalogGauge>(nameof(GraduationThickness), DEFAULT_GRADUATION_THICKNESS);

        public static readonly DependencyProperty GraduationLengthProperty = DP.RegisterProperty<AnalogGauge>(nameof(GraduationLength), DEFAULT_GRADUATION_LENGTH);

        public static readonly DependencyProperty GraduationBrushProperty = DP.RegisterProperty<AnalogGauge>(nameof(GraduationBrush), DEFAULT_GRADUATION_BRUSH);

        public static readonly DependencyProperty LabelIntervalProperty = DP.RegisterProperty<AnalogGauge>(nameof(LabelInterval), DEFAULT_LABEL_INTERVAL);

        public static readonly DependencyProperty LabelIntervalOffsetProperty = DP.RegisterProperty<AnalogGauge>(nameof(LabelIntervalOffset), DEFAULT_LABEL_INTERVAL_OFFSET);

        public static readonly DependencyProperty LabelLevelingProperty = DP.RegisterProperty<AnalogGauge>(nameof(LabelLeveling), DEFAULT_LABEL_LEVELING);

        public static readonly DependencyProperty LabelStyleProperty = DP.RegisterProperty<AnalogGauge>(nameof(LabelStyle));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set
            {
                SetValue(MinimumProperty, value);
                BuildGraduations();
            }
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set
            {
                SetValue(MaximumProperty, value);
                BuildGraduations();
            }
        }

        public double AngleSpan
        {
            get => (double)GetValue(AngleSpanProperty);
            set
            {
                SetValue(AngleSpanProperty, value);
                BuildGraduations();
            }
        }

        public double AngleGap { get => 360 - AngleSpan; }

        public double HorizontalAngleOffset { get => AngleGap / 2; }

        public double MinimumAngle { get => HorizontalAngleOffset - 90; }

        public double MaximumAngle { get => 270 - HorizontalAngleOffset; }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, ApplyNeedleAngle(value));
        }

        public Control NeedleControlElement
        {
            get => (Control)GetValue(NeedleControlElementProperty);
            set => SetValue(NeedleControlElementProperty, value);
        }

        public double NeedleAngle
        {
            get => (double)GetValue(NeedleAngleProperty);
            set => SetValue(NeedleAngleProperty, value);
        }

        public DoubleAnimationGenerator Animator
        {
            get => (DoubleAnimationGenerator)GetValue(AnimatorProperty);
            set => SetValue(AnimatorProperty, value);
        }

        public double OutlineThickness
        {
            get => (double)GetValue(OutlineThicknessProperty);
            set => SetValue(OutlineThicknessProperty, value);
        }

        public Brush OutlineBrush
        {
            get => (Brush)GetValue(OutlineBrushProperty);
            set => SetValue(OutlineBrushProperty, value);
        }

        public double Pitch
        {
            get => (double)GetValue(PitchProperty);
            set
            {
                SetValue(PitchProperty, value);
                BuildGraduations();
            }
        }

        public double GraduationThickness
        {
            get => (double)GetValue(GraduationThicknessProperty);
            set
            {
                SetValue(GraduationThicknessProperty, value);
                BuildGraduations();
            }
        }

        public double GraduationLength
        {
            get => (double)GetValue(GraduationLengthProperty);
            set
            {
                SetValue(GraduationLengthProperty, value);
                BuildGraduations();
            }
        }

        public Brush GraduationBrush
        {
            get => (Brush)GetValue(GraduationBrushProperty);
            set
            {
                SetValue(GraduationBrushProperty, value);
                BuildGraduations();
            }
        }

        public int LabelInterval
        {
            get => (int)GetValue(LabelIntervalProperty);
            set
            {
                SetValue(LabelIntervalProperty, value);
                BuildGraduations();
            }
        }

        public int LabelIntervalOffset
        {
            get => (int)GetValue(LabelIntervalOffsetProperty);
            set
            {
                SetValue(LabelIntervalOffsetProperty, value);
                BuildGraduations();
            }
        }

        public LabelLevel LabelLeveling
        {
            get => (LabelLevel)GetValue(LabelLevelingProperty);
            set
            {
                SetValue(LabelLevelingProperty, value);
                BuildGraduations();
            }
        }

        public Style LabelStyle
        {
            get => (Style)GetValue(LabelStyleProperty);
            set
            {
                SetValue(LabelStyleProperty, value);
                BuildGraduations();
            }
        }


        private Func<double, string> _labelFormatter = (double value) => value.ToString();

        public Func<double, string> LabelFormatter
        {
            get => _labelFormatter;
            set
            {
                _labelFormatter = value;
                BuildGraduations();
            }
        }

        public int GraduationCount
        {
            get => Math.Min(Pitch == 0 ? 0 : (int)Math.Ceiling((Maximum - Minimum) / Pitch) + 1, 100);
        }

        LinearMap _linearMap = new();

        LinearMap NeedleAngleMap
        {
            get
            {
                if (_linearMap.InputInterval.Low != Minimum)
                    _linearMap.InputInterval.Low = Minimum;

                if (_linearMap.InputInterval.High != Maximum)
                    _linearMap.InputInterval.High = Maximum;

                if (_linearMap.OutputInterval.Low != MinimumAngle)
                    _linearMap.OutputInterval.Low = MinimumAngle;

                if (_linearMap.OutputInterval.High != MaximumAngle)
                    _linearMap.OutputInterval.High = MaximumAngle;

                return _linearMap;
            }
        }

        class Graduation
        {
            public DockPanel DockPanel;

            public Rectangle Line;

            public TextBlock Label = new();

            public Graduation(AnalogGauge gauge)
            {
                DockPanel = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Line = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Line.SetBinding(WidthProperty, new Binding() { Source = gauge, Path = new PropertyPath(nameof(gauge.GraduationLength)) });
                Line.SetBinding(HeightProperty, new Binding() { Source = gauge, Path = new PropertyPath(nameof(gauge.GraduationThickness)) });
                Line.SetBinding(Rectangle.FillProperty, new Binding() { Source = gauge, Path = new PropertyPath(nameof(gauge.GraduationBrush)) });
                DockPanel.Children.Add(Line);
                DockPanel.SetDock(Line, Dock.Left);
                Label.SetBinding(StyleProperty, new Binding() { Source = gauge, Path = new PropertyPath(nameof(gauge.LabelStyle)) });
            }
        }

        private List<Graduation> _graduations = new();

        public AnalogGauge()
        {
            DataContext = this;
            InitializeComponent();
            Loaded += (sender, e) =>
            {
                if (NeedleControlElement is null)
                    NeedleControlElement = DEFAULT_NEEDLE_CONTROL_GENERATOR();
                if (LabelStyle is null)
                    LabelStyle = DEFAULT_LABEL_STYLE_GENERATOR();
                ApplyNeedleAngle(BuildGraduations(Value));
            };
        }

        public void AnimateBreak(double value, SweepDirection direction)
        {
            BeginAnimation(
                NeedleAngleProperty,
                Animator(
                    NeedleAngleMap.Map(MaximumAngle) - (direction == SweepDirection.Clockwise ? 360 : 0),
                    NeedleAngleMap.Map(MaximumAngle) - (direction == SweepDirection.Counterclockwise ? 360 : 0)
                ),
                HandoffBehavior.SnapshotAndReplace
            );
            SetValue(ValueProperty, value);
        }

        public T AnimateWith<T>(DoubleAnimationGenerator generator, Func<T> action)
        {
            DoubleAnimationGenerator old = Animator;
            Animator = generator;
            T result = action();
            Animator = old;
            return result;
        }

        public void AnimateWith(DoubleAnimationGenerator generator, Action action) => AnimateWith<object?>(generator, () => { action(); return null; });

        private void ApplyGraduationProperties(Graduation graduation, int index)
        {
            double value = LinearMap.Map(0, _graduations.Count - 1, Minimum, Maximum, index);
            double angle = LinearMap.Map(0, _graduations.Count - 1, MinimumAngle, MaximumAngle, index);
            if (!_graduationsGrid.Children.Contains(graduation.DockPanel))
                _graduationsGrid.Children.Add(graduation.DockPanel);

            if (LabelInterval != 0 && (index - LabelIntervalOffset) % LabelInterval == 0)
            {
                graduation.Label.Text = LabelFormatter(value);
                if (!graduation.DockPanel.Children.Contains(graduation.Label))
                {
                    graduation.DockPanel.Children.Add(graduation.Label);
                    DockPanel.SetDock(graduation.Label, Dock.Right);
                    graduation.DockPanel.UpdateLayout();
                }
                
                switch (LabelLeveling)
                {
                    case LabelLevel.Leveled:
                        graduation.Label.LayoutTransform = new RotateTransform(-angle, graduation.Label.ActualWidth / 2, graduation.Label.ActualHeight / 2);
                        break;
                    case LabelLevel.AngledUpright:
                        double textAngle;
                        if (-45 <= angle && angle <= 45)
                            textAngle = 0;
                        else if (45 < angle && angle < 135)
                            textAngle = -90;
                        else if (135 <= angle && angle <= 225)
                            textAngle = -180;
                        else
                            textAngle = -270;
                        graduation.Label.LayoutTransform = new RotateTransform(textAngle, graduation.Label.ActualWidth / 2, graduation.Label.ActualHeight / 2);
                        break;
                    case LabelLevel.NoLeveling:
                    default:
                        break;
                }
            }
            else if (graduation.DockPanel.Children.Contains(graduation.Label))
            {
                graduation.DockPanel.Children.Remove(graduation.Label);
            }

            graduation.DockPanel.UpdateLayout();
            graduation.DockPanel.RenderTransform = new RotateTransform(angle, GAUGE_CENTER, graduation.DockPanel.ActualHeight / 2);
        }

        protected void BuildGraduations()
        {
            if (!IsLoaded)
                return;

            if (GraduationCount < _graduations.Count)
                _graduations.RemoveRange(GraduationCount, _graduations.Count - GraduationCount);
            else if (GraduationCount > _graduations.Count)
                for (int i = _graduations.Count; i < GraduationCount; i++)
                    _graduations.Add(new(this));

            _graduationsGrid.Children.Clear();
            for (int i = 0; i < _graduations.Count; i++)
                ApplyGraduationProperties(_graduations[i], i);
        }

        protected T BuildGraduations<T>(T value)
        {
            BuildGraduations();
            return value;
        }

        protected double ApplyNeedleAngle(double value)
        {
            BeginAnimation(
                NeedleAngleProperty,
                Animator(
                    NeedleAngleMap.Map(Value),
                    NeedleAngleMap.Map(value)
                ),
                HandoffBehavior.SnapshotAndReplace
            );
            return value;
        }
    }
}
