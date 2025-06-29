using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFGauges.Animations;
using WPFGauges.Data;

namespace WPFGauges.Tape
{
    /// <summary>
    /// Interaction logic for TapeGauge.xaml
    /// </summary>
    public partial class TapeGauge : UserControl
    {
        public static readonly double GAUGE_HEIGHT = 100;

        public static readonly double GAUGE_WIDTH = 30;

        public static readonly double DEFAULT_MINIMUM = 0;

        public static readonly double DEFAULT_MAXIMUM = 100;

        public static readonly double DEFAULT_VALUE = 0;

        public static readonly ThicknessAnimationGenerator DEFAULT_ANIMATOR = Generators.ThicknessAnimation.None;

        public static readonly double DEFAULT_PITCH_HEIGHT = 30;

        public static readonly double DEFAULT_PITCH = 10;

        public static readonly Side DEFAULT_GRADUATION_SIDE = Side.Left;

        public static readonly double DEFAULT_GRADUATION_HEIGHT = 1;

        public static readonly double DEFAULT_GRADUATION_WIDTH = 5;

        public static readonly Brush DEFAULT_GRADUATION_BRUSH = Brushes.Black;

        public static readonly Func<Style> DEFAULT_LABEL_STYLE_GENERATOR = () => new()
        {
            Setters =
            {
                new Setter(TextBlock.FontSizeProperty, 7d),
                new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center)
            }
        };

        public static readonly int DEFAULT_LABEL_INTERVAL = 1;

        public static readonly int DEFAULT_LABEL_INTERVAL_OFFSET = 0;

        public static readonly Func<double, string> DEFAULT_FORMATTER = (x) => x.ToString();

        public static readonly DependencyProperty MinimumProperty = DP.RegisterProperty<TapeGauge>(nameof(Minimum), DEFAULT_MINIMUM);

        public static readonly DependencyProperty MaximumProperty = DP.RegisterProperty<TapeGauge>(nameof(Maximum), DEFAULT_MAXIMUM);

        public static readonly DependencyProperty ValueProperty = DP.RegisterProperty<TapeGauge>(nameof(Value), DEFAULT_VALUE);

        public static readonly DependencyProperty AnimatorProperty = DP.RegisterProperty<TapeGauge>(nameof(Animator), DEFAULT_ANIMATOR);

        public static readonly DependencyProperty PitchHeightProperty = DP.RegisterProperty<TapeGauge>(nameof(PitchHeight), DEFAULT_PITCH_HEIGHT);

        public static readonly DependencyProperty PitchProperty = DP.RegisterProperty<TapeGauge>(nameof(Pitch), DEFAULT_PITCH);

        public static readonly DependencyProperty GraduationSideProperty = DP.RegisterProperty<TapeGauge>(nameof(GraduationSide), DEFAULT_GRADUATION_SIDE);

        public static readonly DependencyProperty GraduationHeightProperty = DP.RegisterProperty<TapeGauge>(nameof(GraduationHeight), DEFAULT_GRADUATION_HEIGHT);

        public static readonly DependencyProperty GraduationWidthProperty = DP.RegisterProperty<TapeGauge>(nameof(GraduationWidth), DEFAULT_GRADUATION_WIDTH);

        public static readonly DependencyProperty GraduationBrushProperty = DP.RegisterProperty<TapeGauge>(nameof(GraduationBrush), DEFAULT_GRADUATION_BRUSH);

        public static readonly DependencyProperty LabelStyleProperty = DP.RegisterProperty<TapeGauge>(nameof(LabelStyle));

        public static readonly DependencyProperty LabelIntervalProperty = DP.RegisterProperty<TapeGauge>(nameof(LabelInterval), DEFAULT_LABEL_INTERVAL);

        public static readonly DependencyProperty LabelIntervalOffsetProperty = DP.RegisterProperty<TapeGauge>(nameof(LabelIntervalOffset), DEFAULT_LABEL_INTERVAL_OFFSET);

        public static readonly DependencyProperty FormatterProperty = DP.RegisterProperty<TapeGauge>(nameof(Formatter), DEFAULT_FORMATTER);

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

        public double MinimumTopMargin
        {
            get => -(PitchHeight * (GraduationCount - 1)) + (GAUGE_HEIGHT / 2) - PitchHeight / 2;
        }

        public double MaximumTopMargin
        {
            get => (GAUGE_HEIGHT / 2) - PitchHeight / 2;
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, ApplyValue(value));
        }

        public ThicknessAnimationGenerator Animator
        {
            get => (ThicknessAnimationGenerator)GetValue(AnimatorProperty);
            set => SetValue(AnimatorProperty, value);
        }

        public double PitchHeight
        {
            get => (double)GetValue(PitchHeightProperty);
            set
            {
                SetValue(PitchHeightProperty, value);
                BuildGraduations();
            }
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

        public Side GraduationSide
        {
            get => (Side)GetValue(GraduationSideProperty);
            set
            {
                SetValue(GraduationSideProperty, value);
                BuildGraduations();
            }
        }

        public double GraduationHeight
        {
            get => (double)GetValue(GraduationHeightProperty);
            set => SetValue(GraduationHeightProperty, value);
        }

        public double GraduationWidth
        {
            get => (double)GetValue(GraduationWidthProperty);
            set => SetValue(GraduationWidthProperty, value);
        }

        public Brush GraduationBrush
        {
            get => (Brush)GetValue(GraduationBrushProperty);
            set => SetValue(GraduationBrushProperty, value);
        }

        public int GraduationCount
        {
            get => Pitch == 0 ? 0 : (int)Math.Ceiling((Maximum - Minimum) / Pitch) + 1;
        }

        public Style LabelStyle
        {
            get => (Style)GetValue(LabelStyleProperty);
            set => SetValue(LabelStyleProperty, value);
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

        public Func<double, string> Formatter
        {
            get => (Func<double, string>)GetValue(FormatterProperty);
            set
            {
                SetValue(FormatterProperty, value);
                BuildGraduations();
            }
        }

        readonly LinearMap _linearMap = new();

        LinearMap TopMarginMap
        {
            get
            {
                if (_linearMap.InputInterval.Low != Minimum)
                    _linearMap.InputInterval.Low = Minimum;

                if (_linearMap.InputInterval.High != Maximum)
                    _linearMap.InputInterval.High = Maximum;

                if (_linearMap.OutputInterval.Low != MinimumTopMargin)
                    _linearMap.OutputInterval.Low = MinimumTopMargin;

                if (_linearMap.OutputInterval.High != MaximumTopMargin)
                    _linearMap.OutputInterval.High = MaximumTopMargin;

                return _linearMap;
            }
        }

        private class Graduation
        {
            public DockPanel Dock;

            public TextBlock Label;

            public Rectangle Line;

            public Graduation(TapeGauge gauge)
            {
                Dock = new();
                Label = new();
                Line = new()
                {
                    VerticalAlignment = VerticalAlignment.Center
                };
                Line.SetBinding(Rectangle.FillProperty, new Binding(nameof(gauge.GraduationBrush)) { Source = gauge });
                Line.SetBinding(WidthProperty, new Binding(nameof(gauge.GraduationWidth)) { Source = gauge });
                Line.SetBinding(HeightProperty, new Binding(nameof(gauge.GraduationHeight)) { Source = gauge });
                Dock.Children.Add(Line);

                Label.VerticalAlignment = VerticalAlignment.Center;
                Label.SetBinding(StyleProperty, new Binding(nameof(gauge.LabelStyle)) { Source = gauge });

                Dock.HorizontalAlignment = HorizontalAlignment.Stretch;
                Dock.SetBinding(HeightProperty, new Binding(nameof(gauge.PitchHeight)) { Source = gauge });
            }
        }

        readonly List<Graduation> _graduations = [];

        public TapeGauge()
        {
            DataContext = this;
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                LabelStyle ??= DEFAULT_LABEL_STYLE_GENERATOR();
                BuildGraduations();
                ApplyValue(Value);
            };
        }

        public Thickness MarginForValue(double value)
            => new(0, TopMarginMap.Map(value), 0, 0);

        protected double ApplyValue(double value)
        {
            _tapeStack.BeginAnimation(
                MarginProperty,
                Animator(
                    _tapeStack.Margin,
                    MarginForValue(value)
                )
            );
            return value;
        }

        private void UpdateGraduationProperties(Graduation graduation, int index)
        {
            double value = LinearMap.Map(0, _graduations.Count - 1, Maximum, Minimum, index);
            if (!_tapeStack.Children.Contains(graduation.Dock))
                _tapeStack.Children.Add(graduation.Dock);

            graduation.Line.HorizontalAlignment = GraduationSide.GetHorizontalAlignment();
            DockPanel.SetDock(graduation.Line, GraduationSide.GetDock());

            if (LabelInterval != 0 && (index - LabelIntervalOffset) % LabelInterval == 0)
            {
                graduation.Label.Text = Formatter(value);
                if (!graduation.Dock.Children.Contains(graduation.Label))
                {
                    graduation.Dock.Children.Add(graduation.Label);
                    DockPanel.SetDock(graduation.Label, GraduationSide.Other().GetDock());
                }
            }
            else if (graduation.Dock.Children.Contains(graduation.Label))
            {
                graduation.Dock.Children.Remove(graduation.Label);
            }
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

            _tapeStack.Children.Clear();
            for (int i = 0; i < _graduations.Count; i++)
                UpdateGraduationProperties(_graduations[i], i);
        }
    }
}
