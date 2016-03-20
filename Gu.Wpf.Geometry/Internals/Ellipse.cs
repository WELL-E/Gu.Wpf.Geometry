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

        public Ellipse(Point center, double radiusX, double radiusY)
        {
            this.Center = center;
            this.RadiusX = radiusX;
            this.RadiusY = radiusY;
        }

        public bool IsZero => this.RadiusX <= 0 || this.RadiusY <= 0;
    }
}