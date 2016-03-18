namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    [DebuggerDisplay("{Center.ToString()}, Radius: {Radius}")]
    internal struct Circle
    {
        internal readonly Point Center;
        internal readonly double Radius;

        public Circle(Point center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        internal static Circle Parse(string text)
        {
            var strings = text.Split(';');
            if (strings.Length != 2)
            {
                throw new ArgumentException();
            }

            var cp = Point.Parse(strings[0]);
            var r = double.Parse(strings[1]);
            return new Circle(cp, r);
        }

        internal Point? ClosestIntersection(Line line)
        {
            var perp = line.PerpendicularLineTo(this.Center);
            var pl = perp.Length;
            if (pl < 1E-3)
            {
                return this.Center - this.Radius * line.Direction;
            }

            if (pl > this.Radius)
            {
                return null;
            }

            var tangentLength = Math.Sqrt(this.Radius * this.Radius - pl * pl);
            return perp.StartPoint - tangentLength * line.Direction;
        }
    }
}
