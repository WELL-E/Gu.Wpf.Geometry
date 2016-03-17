namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public class NullablePointComparer : IEqualityComparer<Point?>
    {
        public static readonly NullablePointComparer Default = new NullablePointComparer();

        public bool Equals(Point? x, Point? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return PointComparer.Default.Equals(x.Value, y.Value);
        }

        public int GetHashCode(Point? obj)
        {
            return obj?.Round().GetHashCode() ?? 0;
        }
    }
}