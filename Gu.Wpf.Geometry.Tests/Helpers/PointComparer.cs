﻿namespace Gu.Wpf.Geometry.Tests
{
    using System.Collections.Generic;
    using System.Windows;

    public class PointComparer : IEqualityComparer<Point>
    {
        private readonly int digits;
        public static readonly PointComparer Default = new PointComparer(2);

        public PointComparer(int digits)
        {
            this.digits = digits;
        }

        public bool Equals(Point x, Point y)
        {
            return this.Equals(x, y, this.digits);
        }

        public bool Equals(Point x, Point y, int decimalDigits)
        {
            return x.Round(decimalDigits) == y.Round(decimalDigits);
        }

        public int GetHashCode(Point obj)
        {
            return obj.Round(this.digits).GetHashCode();
        }
    }
}