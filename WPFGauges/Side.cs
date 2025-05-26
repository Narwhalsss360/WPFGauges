using System.Windows;
using System.Windows.Controls;

namespace WPFGauges
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

        public static HorizontalAlignment GetHorizontalAlignment(this Side side)
            => side == Side.Left ? HorizontalAlignment.Left : HorizontalAlignment.Right;

        public static Dock GetDock(this Side side)
            => side == Side.Left ? Dock.Left : Dock.Right;
    }
}
