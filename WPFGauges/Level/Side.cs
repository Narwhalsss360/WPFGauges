namespace WPFGauges.Level
{
    public enum Side
    {
        Left,
        Right
    }

    public static class SideMethods
    {
        public static Side Other(this Side side)
            => side == Side.Left ? Side.Right : Side.Left;
    }
}
