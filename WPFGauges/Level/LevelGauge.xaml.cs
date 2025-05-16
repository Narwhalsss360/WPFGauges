using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFGauges.Animations;
using WPFGauges.Data;

namespace WPFGauges.Level
{
    /// <summary>
    /// Interaction logic for LevelGauge.xaml
    /// </summary>
    public partial class LevelGauge : UserControl
    {
        public static readonly double GAUGE_HEIGHT = 100;

        public static readonly double GAUGE_WIDTH = 30;

        public static readonly double GAUGE_HORIZONTAL_CENTER = GAUGE_HEIGHT / 2;

        public static readonly double GAUGE_VERTICAL_CENTER = GAUGE_WIDTH / 2;

        public static readonly double DEFAULT_MINIMUM = 0;

        public static readonly double DEFAULT_MAXIMUM = 100;

        public static readonly double DEFAULT_VALUE = 0;

        public static readonly double DEFAULT_LEVEL = 0;

        public static readonly double DEFAULT_LEVEL_WIDTH = 10;

        public static readonly double DEFAULT_OUTLINE_THICKNESS = 1;

        public static readonly Brush DEFAULT_OUTLINE_BRUSH = Brushes.Black;

        protected static readonly Thickness DEFAULT_LEVEL_MARGIN = LevelMarginForOutlineThickness(DEFAULT_OUTLINE_THICKNESS);

        public static readonly DoubleAnimationGenerator DEFAULT_ANIMATOR = (from, to) => new(from, to, TimeSpan.Zero);

        public static readonly double DEFAULT_PITCH = 0;

        public static readonly double DEFAULT_GRADUATION_THICKNESS = 0.5;

        public static readonly double DEFAULT_GRADUATION_LENGTH = 2;

        public static readonly Brush DEFAULT_GRADUATION_BRUSH = Brushes.Black;

        public static readonly Side DEFAULT_GRADUATION_START_SIDE = Side.Right;

        public static readonly bool DEFAULT_ALTERNATE_GRADUATIONS = false;

        public static readonly int DEFAULT_LABEL_INTERVAL = 0;

        public static readonly int DEFAULT_LABEL_INTERVAL_OFFSET = 0;

        public static readonly Func<Style> DEFAULT_LABEL_STYLE_GENERATOR = () => new()
        {
            Setters =
            {
                new Setter(TextBlock.FontSizeProperty, 2.25d),
                new Setter(VerticalAlignmentProperty, VerticalAlignment.Center),
                new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center)
            }
        };

        public static readonly double MINIMUM_LEVEL = 0;

        public static readonly double MAXIMUM_LEVEL = GAUGE_HEIGHT;

        public static readonly DependencyProperty MinimumProperty = DP.RegisterProperty<LevelGauge>(nameof(Minimum), DEFAULT_MINIMUM);

        public static readonly DependencyProperty MaximumProperty = DP.RegisterProperty<LevelGauge>(nameof(Maximum), DEFAULT_MAXIMUM);

        public static readonly DependencyProperty ValueProperty = DP.RegisterProperty<LevelGauge>(nameof(Value), DEFAULT_VALUE);

        public static readonly DependencyProperty LevelProperty = DP.RegisterProperty<LevelGauge>(nameof(Level), DEFAULT_LEVEL);

        public static readonly DependencyProperty LevelWidthProperty = DP.RegisterProperty<LevelGauge>(nameof(LevelWidth), DEFAULT_LEVEL_WIDTH);

        public static readonly DependencyProperty OutlineThicknessProperty = DP.RegisterProperty<LevelGauge>(nameof(OutlineThickness), DEFAULT_OUTLINE_THICKNESS);

        public static readonly DependencyProperty OutlineBrushProperty = DP.RegisterProperty<LevelGauge>(nameof(OutlineBrush), DEFAULT_OUTLINE_BRUSH);

        protected static readonly DependencyProperty LevelMarginProperty = DP.RegisterProperty<LevelGauge>(nameof(LevelMargin), DEFAULT_LEVEL_MARGIN);

        public static readonly DependencyProperty AnimatorProperty = DP.RegisterProperty<LevelGauge>(nameof(Animator), DEFAULT_ANIMATOR);

        public static readonly DependencyProperty PitchProperty = DP.RegisterProperty<LevelGauge>(nameof(Pitch), DEFAULT_PITCH);

        public static readonly DependencyProperty GraduationThicknessProperty = DP.RegisterProperty<LevelGauge>(nameof(GraduationThickness), DEFAULT_GRADUATION_THICKNESS);

        public static readonly DependencyProperty GraduationLengthProperty = DP.RegisterProperty<LevelGauge>(nameof(GraduationLength), DEFAULT_GRADUATION_LENGTH);

        public static readonly DependencyProperty GraduationBrushProperty = DP.RegisterProperty<LevelGauge>(nameof(GraduationBrush), DEFAULT_GRADUATION_BRUSH);
        
        public static readonly DependencyProperty GraduationStartSideProperty = DP.RegisterProperty<LevelGauge>(nameof(GraduationStartSide), DEFAULT_GRADUATION_START_SIDE);

        public static readonly DependencyProperty AlternateGraduationsProperty = DP.RegisterProperty<LevelGauge>(nameof(AlternateGraduations), DEFAULT_ALTERNATE_GRADUATIONS);

        public static readonly DependencyProperty LabelIntervalProperty = DP.RegisterProperty<LevelGauge>(nameof(LabelInterval), DEFAULT_LABEL_INTERVAL);

        public static readonly DependencyProperty LabelIntervalOffsetProperty = DP.RegisterProperty<LevelGauge>(nameof(LabelIntervalOffset), DEFAULT_LABEL_INTERVAL_OFFSET);

        public static readonly DependencyProperty LabelStyleProperty = DP.RegisterProperty<LevelGauge>(nameof(LabelStyle));

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

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, ApplyLevel(value));
        }

        public double Level
        {
            get => (double)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }

        public double LevelWidth
        {
            get => (double)GetValue(LevelWidthProperty);
            set => SetValue(LevelWidthProperty, value);
        }

        public double OutlineThickness
        {
            get => (double)GetValue(OutlineThicknessProperty);
            set
            {
                SetValue(OutlineThicknessProperty, value);
                LevelMargin = LevelMarginForOutlineThickness(value);
                BuildGraduations();
            }
        }

        public Brush OutlineBrush
        {
            get => (Brush)GetValue(OutlineBrushProperty);
            set => SetValue(OutlineBrushProperty, value);
        }

        public Thickness LevelMargin
        {
            get => (Thickness)GetValue(LevelMarginProperty);
            protected set => SetValue(LevelMarginProperty, value);
        }

        public DoubleAnimationGenerator Animator
        {
            get => (DoubleAnimationGenerator)GetValue(AnimatorProperty);
            set => SetValue(AnimatorProperty, value);
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

        public Side GraduationStartSide
        {
            get => (Side)GetValue(GraduationStartSideProperty);
            set
            {
                SetValue(GraduationStartSideProperty, value);
                BuildGraduations();
            }
        }

        public bool AlternateGraduations
        {
            get => (bool)GetValue(AlternateGraduationsProperty);
            set
            {
                SetValue(AlternateGraduationsProperty, value);
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

        public Style LabelStyle
        {
            get => (Style)GetValue(LabelStyleProperty);
            set
            {
                SetValue(LabelStyleProperty, value);
                BuildGraduations();
            }
        }

        LinearMap _linearMap = new();

        LinearMap LevelMap
        {
            get
            {
                if (_linearMap.InputInterval.Low != Minimum)
                    _linearMap.InputInterval.Low = Minimum;

                if (_linearMap.InputInterval.High != Maximum)
                    _linearMap.InputInterval.High = Maximum;

                if (_linearMap.OutputInterval.Low != MINIMUM_LEVEL)
                    _linearMap.OutputInterval.Low = MINIMUM_LEVEL;

                if (_linearMap.OutputInterval.High != MAXIMUM_LEVEL)
                    _linearMap.OutputInterval.High = MAXIMUM_LEVEL;

                return _linearMap;
            }
        }

        public int GraduationCount
        {
            get => Math.Min(Pitch == 0 ? 0 : (int)Math.Ceiling((Maximum - Minimum) / Pitch) + 1, 100);
        }

        class Graduation
        {
            public DockPanel DockPanel;

            public Rectangle Line;

            public TextBlock Label;

            public Graduation(LevelGauge gauge)
            {
                DockPanel = new()
                {
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                Line = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Label = new();

                Line.SetBinding(WidthProperty, new Binding() { Source = gauge, Path = new PropertyPath(nameof(gauge.GraduationLength)) });
                Line.SetBinding(HeightProperty, new Binding() { Source = gauge, Path = new PropertyPath(nameof(gauge.GraduationThickness)) });
                Line.SetBinding(Rectangle.FillProperty, new Binding() { Source = gauge, Path = new PropertyPath(nameof(gauge.GraduationBrush)) });
                DockPanel.Children.Add(Line);
                Label.SetBinding(StyleProperty, new Binding() { Source = gauge, Path = new PropertyPath(nameof(gauge.LabelStyle)) });
            }
        }

        List<Graduation> _graduations = new();

        public LevelGauge()
        {
            if (LabelStyle is null)
                LabelStyle = DEFAULT_LABEL_STYLE_GENERATOR();
            DataContext = this;
            InitializeComponent();
            Loaded += (sender, e) =>
            {
                LevelMargin = LevelMarginForOutlineThickness(OutlineThickness);
                ApplyLevel(BuildGraduations(Value));
            };
        }

        private void ApplyGraduationProperties(Graduation graduation, int index)
        {
            double value = LinearMap.Map(0, _graduations.Count - 1, Minimum, Maximum, index);
            double bottomMargin = LinearMap.Map(0, _graduations.Count - 1, MINIMUM_LEVEL + OutlineThickness, MAXIMUM_LEVEL - OutlineThickness, index);
            bool left = GraduationStartSide == Side.Left;
            if (AlternateGraduations)
                left = (index % 2 == 0 ? GraduationStartSide : GraduationStartSide.Other()) == Side.Left;

            if (!_graduationsGrid.Children.Contains(graduation.DockPanel))
                _graduationsGrid.Children.Add(graduation.DockPanel);

            graduation.DockPanel.HorizontalAlignment = left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            DockPanel.SetDock(graduation.Line, left? Dock.Right : Dock.Left);

            if (LabelInterval != 0 && (index - LabelIntervalOffset) % LabelInterval == 0)
            {
                graduation.Label.Text = LabelFormatter(value);
                if (!graduation.DockPanel.Children.Contains(graduation.Label))
                {
                    graduation.DockPanel.Children.Add(graduation.Label);
                    DockPanel.SetDock(graduation.Label, left ? Dock.Left : Dock.Right);
                }
            }
            else if (graduation.DockPanel.Children.Contains(graduation.Label))
            {
                graduation.DockPanel.Children.Remove(graduation.Label);
            }

            Grid.SetColumn(graduation.DockPanel, left ? 0 : 2);
            graduation.DockPanel.UpdateLayout();
            bottomMargin -= graduation.DockPanel.ActualHeight / 2;
            graduation.DockPanel.Margin = new(0, 0, 0, bottomMargin);
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

        protected double ApplyLevel(double value)
        {
            BeginAnimation(
                LevelProperty,
                Animator(
                    LevelMap.Map(Value),
                    LevelMap.Map(value)
                ),
                HandoffBehavior.SnapshotAndReplace
            );
            return value;
        }

        private static Thickness LevelMarginForOutlineThickness(double thickness)
            => new(0, 0, 0, thickness);
    }
}
