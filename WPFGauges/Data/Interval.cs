namespace WPFGauges.Data
{
    public struct Interval
    {
        public double Low;

        public double High;

        public bool Degenerate { get => Low == High; }

        public Interval(double low, double high)
        {
            Low = low;
            High = high;
        }

        public Interval()
            : this(0, 0)
        { }
    }
}
