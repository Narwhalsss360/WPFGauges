namespace WPFGauges.Data
{
    public struct Interval(double low, double high)
    {
        public double Low = low;

        public double High = high;

        public readonly bool Degenerate { get => Low == High; }

        public Interval()
            : this(0, 0)
        { }
    }
}
