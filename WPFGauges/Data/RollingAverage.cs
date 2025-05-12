namespace WPFGauges.Data
{
    public class RollingAverage
    {
        public delegate double AverageFunction(IEnumerable<double> values);

        double[] _values = new double[1];

        public int AverageCount
        {
            get { return _values.Length; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Must average at least 1 value.", nameof(AverageCount));

                double[] values = new double[value];
                Array.Copy(_values, values, Math.Min(_values.Length, value));
                if (value < _values.Length)
                {
                    _filled = Math.Min(_filled, value);
                    _position = Math.Min(_position, value);
                }
                _values = values;
            }
        }

        private double _current;

        public double Current
        {
            get => _current;
        }

        public AverageFunction Averager { get; set; } = Enumerable.Average;

        int _filled = 0;

        int _position = 0;

        public RollingAverage()
        {
        }

        public RollingAverage(int averageCount)
        {
            AverageCount = averageCount;
        }

        public double Next(double value)
        {
            _values[_position++] = value;
            if (_filled != _values.Length)
                _filled++;
            _position %= AverageCount;
            _current = Averager(_values.Take(_filled));
            return _current;
        }
    }
}
