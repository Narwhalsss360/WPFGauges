using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;
using WPFGauges.Analog;
using WPFGauges.Animations;
using WPFGauges.Data;
using WPFGauges.Level;

namespace BasicPerformanceGauges;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static readonly int REFRESH_RATE = 2;

    public static readonly TimeSpan REFRESH_INTERVAL = TimeSpan.FromMilliseconds(1000.0 / REFRESH_RATE);

    readonly System.Timers.Timer _refreshTimer = new(REFRESH_INTERVAL.TotalMilliseconds) { AutoReset = true };

    readonly List<Action> _gaugeUpdateActions = [];

    bool _updating = false;

    readonly DoubleAnimationGenerator _doubleAnimator = (from, to) => new(from, to, REFRESH_INTERVAL);

    readonly ThicknessAnimationGenerator _thicknessAnimator = (from, to) => new(from, to, REFRESH_INTERVAL);

    readonly Func<double, string> _percentGaugeFormatter = (double value) => $"{value}%";

    public MainWindow()
    {
        InitializeComponent();

        SetupGauge(_refreshTimerGauge, () => Dispatcher.Invoke(() => 
        {
            _refreshTimerGauge.Value = _refreshTimerGauge.Value == _refreshTimerGauge.Minimum ? _refreshTimerGauge.Maximum : _refreshTimerGauge.Minimum;
        }));
        SetupBubbleRefreshTimerGauge();

        _cpuUsageLevel.Animator = _doubleAnimator;
        Task[] setups =
        [
            Task.Run(() =>
            {
                AnalogGauge gauge = _cpuUsageGauge;
                RollingAverage rollingAverage = new(REFRESH_RATE / 2);
                PerformanceCounter counter = new("Processor", "% Processor Time", "_Total");
                SetupPercentGauge(gauge, () => Dispatcher.Invoke(() =>
                {
                    gauge.Value = rollingAverage.Next(counter.NextValue());
                    _cpuUsageLevel.Value = rollingAverage.Current;
                    _cpuUsageTape.Value = rollingAverage.Current;
                }));
            }),
            Task.Run(() =>
            {
                AnalogGauge gauge = _ramUsageGauge;
                RollingAverage rollingAverage = new(REFRESH_RATE * 3);
                PerformanceCounter counter = new("Memory", "% Committed Bytes In Use");
                SetupPercentGauge(gauge, () => Dispatcher.Invoke(() => gauge.Value = rollingAverage.Next(counter.NextValue())));
            }),
            Task.Run(() =>
            {
                AnalogGauge gauge = _diskBytesGauge;
                RollingAverage rollingAverage = new(REFRESH_RATE * 3);
                PerformanceCounter counter = new("PhysicalDisk", "disk bytes/sec", "_total");
                SetupSpeedGauge(gauge, () => rollingAverage.Next(counter.NextValue()));
            })
        ];

        _diskBytesGauge.LabelFormatter = (double bytes) => $"{Math.Round(bytes / 1024)} KB";
        _diskBytesGauge.Animator = (from, to) => new(from, to, REFRESH_INTERVAL) { EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 2 } };
        Loaded += (sender, e) => Task.WhenAll(setups).ContinueWith(task =>
        {
            if (task.IsFaulted)
                throw task.Exception;
            _refreshTimer.Start();
        });
        _refreshTimer.Elapsed += RefreshTimerElapsed;

        _cpuUsageTape.Animator = _thicknessAnimator;
    }

    private void SetupBubbleRefreshTimerGauge()
    {
        _refreshTimerBubble.Animator = _doubleAnimator;
        _gaugeUpdateActions.Add(() => Dispatcher.Invoke(() => _refreshTimerBubble.Value = _refreshTimerBubble.Value == -1 ? 1 : -1));
    }

    private void RefreshTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_updating)
            return;
        _updating = true;
        foreach (Action action in _gaugeUpdateActions)
            action();
        _updating = false;
    }

    private void SetupGauge(AnalogGauge gauge, Action updateAction)
    {
        void setup()
        {
            gauge.Animator = _doubleAnimator;
            _gaugeUpdateActions.Add(updateAction);
        }

        if (CheckAccess())
            setup();
        else
            Dispatcher.Invoke(setup);
    }

    private void SetupPercentGauge(AnalogGauge gauge, Action updateAction)
    {
        void setup()
        {
            gauge.Animator = _doubleAnimator;
            gauge.LabelFormatter = _percentGaugeFormatter;
            _gaugeUpdateActions.Add(updateAction);
        }

        if (CheckAccess())
            setup();
        else
            Dispatcher.Invoke(setup);
    }

    private void SetupSpeedGauge(AnalogGauge gauge, Func<double> updateAction)
    {
        void setup()
        {
            gauge.Animator = _doubleAnimator;
            _gaugeUpdateActions.Add(() =>
            {
                double value = updateAction();
                Dispatcher.Invoke(() =>
                {
                    if (value < gauge.Minimum)
                    {
                        gauge.Pitch = (gauge.Maximum - value) / 10;
                        gauge.Minimum = value;
                    }
                    else if (value > gauge.Maximum)
                    {
                        gauge.Pitch = (value - gauge.Minimum) / 10;
                        gauge.Maximum = value;
                    }

                    gauge.Value = value;
                });
            });
        }

        if (CheckAccess())
            setup();
        else
            Dispatcher.Invoke(setup);
    }
}