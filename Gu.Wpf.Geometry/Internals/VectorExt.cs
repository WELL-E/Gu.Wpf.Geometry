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

        internal static double DotProdcut(this Vector v, Vector other)
        {
            return Vector.Multiply(v, other);
        }

        internal static Vector ProjectOn(this Vector v, Vector other)
        {
            var dp = v.DotProdcut(other);
            return dp * other;
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

        internal static Vector Round(this Vector v, int digits = 0)
        {
            return new Vector(Math.Round(v.X, digits), Math.Round(v.Y, digits));
        }
    }
}
