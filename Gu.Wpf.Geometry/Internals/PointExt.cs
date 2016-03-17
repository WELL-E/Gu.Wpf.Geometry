namespace Gu.Wpf.Geometry
{
    using System;
    using System.Globalization;
    using System.Windows;

    internal static class PointExt
    {
        internal static Point WithOffset(this Point self, Vector direction, double distance)
        {
            return self + distance * direction;
        }

        internal static Point WithOffset(this Point self, double x, double y)
        {
            return new Point(self.X + x, self.Y + y);
        }

        internal static double DistanceTo(this Point self, Point other)
        {
            return (self - other).Length;
        }

        internal static Point Round(this Point self, int digits = 0)
        {
            return new Point(Math.Round(self.X, digits), Math.Round(self.Y, digits));
        }

        internal static Vector VectorTo(this Point self, Point other)
        {
            return other - self;
        }

        internal static Point Closest(this Point self, Point first, Point other)
        {
            return self.DistanceTo(first) < self.DistanceTo(other) ? first : other;
        }

        internal static Point MidPoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        internal static string ToString(this Point? self, string format = "F1")
        {
            return self == null ? "null" : self.Value.ToString(format);
        }

        internal static string ToString(this Point self, string format = "F1")
        {
            return $"{self.X.ToString(format, CultureInfo.InvariantCulture)},{self.Y.ToString(format, CultureInfo.InvariantCulture)}";
        }
    }
}
