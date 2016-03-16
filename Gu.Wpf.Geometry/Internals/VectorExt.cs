namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;

    internal static class VectorExt
    {
        private const double DegToRad = Math.PI / 180;

        internal static Vector Rotate(this Vector v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        internal static Vector RotateRadians(this Vector v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        internal static Vector Normalized(this Vector v)
        {
            var uv = new Vector(v.X, v.Y);
            uv.Normalize();
            return uv;
        }

        internal static Vector Negated(this Vector v)
        {
            var negated = new Vector(v.X, v.Y);
            negated.Negate();
            return negated;
        }
    }
}
