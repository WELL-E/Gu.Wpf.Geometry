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
            var v = this.Center - line.StartPoint;
            var projected = v.ProjectOn(line.Direction);
            var pp = line.StartPoint + projected;
            var pv = pp - this.Center;
            var pl = pv.Length;
            if (pl > this.Radius)
            {
                return null;
            }
            if (pl < 1E-3)
            {
                return this.Center + this.Radius*line.Direction.Negated();
            }

            var tl = Math.Sqrt(this.Radius * this.Radius - pl * pl);
            var tv = tl * pv.Rotate(90).Normalized();

            return this.Center + (pv + tv);
        }
    }
}
