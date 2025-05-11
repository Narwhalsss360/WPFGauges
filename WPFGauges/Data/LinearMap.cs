namespace WPFGauges.Data
{
    public class LinearMap : IMap
    {
        public Interval InputInterval;

        public Interval OutputInterval;

        public LinearMap(Interval inputInterval, Interval outputInterval)
        {
            InputInterval = inputInterval;
            OutputInterval = outputInterval;
        }

        public LinearMap()
        { }

        public double Map(double input)
        {
            if (InputInterval.Degenerate)
                throw new InvalidOperationException("The linear map input interval is degenerate");
            if (OutputInterval.Degenerate)
                throw new InvalidOperationException("The linear map output interval is degenerate");

            return (input - InputInterval.Low) * (OutputInterval.High - OutputInterval.Low) / (InputInterval.High - InputInterval.Low) + OutputInterval.Low;
        }

        public static double Map(double inputLow, double inputHigh, double outputLow, double outputHigh, double input)
        {
            return new LinearMap(new(inputLow, inputHigh), new(outputLow, outputHigh)).Map(input);
        }
    }
}
