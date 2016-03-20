namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    internal struct Ellipse
    {
        internal readonly Point Center;
        internal readonly double RadiusX;
        internal readonly double RadiusY;

        internal Ellipse(Rect rect)
        {
            Debug.Assert(!rect.IsEmpty, "!rect.IsEmpty");
            this.RadiusX = (rect.Right - rect.X) * 0.5;
            this.RadiusY = (rect.Bottom - rect.Y) * 0.5;
            this.Center = new Point(rect.X + this.RadiusX, rect.Y + this.RadiusY);
        }

        internal Ellipse(Point center, double radiusX, double radiusY)
        {
            this.Center = center;
            this.RadiusX = radiusX;
            this.RadiusY = radiusY;
        }

        internal bool IsZero => this.RadiusX <= 0 || this.RadiusY <= 0;

        internal static Ellipse Parse(string text)
        {
            var strings = text.Split(';');
            if (strings.Length != 3)
            {
                throw new ArgumentException();
            }

            var cp = Point.Parse(strings[0]);
            var rx = double.Parse(strings[1]);
            var ry = double.Parse(strings[2]);
            return new Ellipse(cp, rx,ry);
        }

        internal Point PointOnCircumference(Vector directionFromCenter)
        {
            var a = new Vector(1, 0).AngleTo(directionFromCenter) * Constants.ToRad;
            var x = this.Center.X + this.RadiusX * Math.Cos(a);
            var y = this.Center.Y + this.RadiusY * Math.Sin(a);
            return new Point(x, y);
        }
    }
}