namespace Gu.Wpf.Geometry
{
    using System;
    using System.Globalization;
    using System.Windows;

    internal static class PointExt
    {
        internal static Point WithOffset(this Point p, Vector direction, double distance)
        {
            return p + distance * direction;
        }

        internal static Point WithOffset(this Point p, double x, double y)
        {
            return new Point(p.X + x, p.Y + y);
        }

        internal static double DistanceTo(this Point p, Point other)
        {
            return (p - other).Length;
        }

        internal static Point Round(this Point p, int digits = 0)
        {
            return new Point(Math.Round(p.X, digits), Math.Round(p.Y, digits));
        }

        internal static Point Closest(this Point p, Point first, Point other)
        {
            return p.DistanceTo(first) < p.DistanceTo(other) ? first : other;
        }

        internal static Point MidPoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        internal static string ToString(this Point? p, string format = "F1")
        {
            return p == null ? "null" : p.Value.ToString(format);
        }

        internal static string ToString(this Point p, string format = "F1")
        {
            return $"{p.X.ToString(format, CultureInfo.InvariantCulture)},{p.Y.ToString(format, CultureInfo.InvariantCulture)}";
        }
    }
}
