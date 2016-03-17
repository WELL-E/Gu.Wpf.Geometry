namespace Gu.Wpf.Geometry
{
    using System.Windows;

    internal static class RectExt
    {
        internal static Line TopLine(this Rect rect)
        {
            return new Line(rect.TopLeft, rect.TopRight);
        }

        internal static Line BottomLine(this Rect rect)
        {
            return new Line(rect.BottomLeft, rect.BottomRight);
        }

        internal static Line LeftLine(this Rect rect)
        {
            return new Line(rect.BottomLeft, rect.TopLeft);
        }

        internal static Line RightLine(this Rect rect)
        {
            return new Line(rect.BottomRight, rect.TopRight);
        }
    }
}
