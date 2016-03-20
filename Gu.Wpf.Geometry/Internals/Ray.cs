namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;

    internal struct Ray
    {
        internal readonly Point Point;
        internal readonly Vector Direction;

        internal Ray(Point point, Vector direction)
        {
            this.Point = point;
            this.Direction = direction;
        }

        internal static Ray Parse(string text)
        {
            var strings = text.Split(';');
            if (strings.Length != 2)
            {
                throw new ArgumentException();
            }

            var p = Point.Parse(strings[0]);
            var v = Vector.Parse(strings[1]).Normalized();
            return new Ray(p, v);
        }

        internal Ray Rotate(double angleInDegrees)
        {
            return new Ray(this.Point, this.Direction.Rotate(angleInDegrees));
        }

        internal bool IsPointOn(Point p)
        {
            if (this.Point.DistanceTo(p) < Constants.Tolerance)
            {
                return true;
            }

            var angle = this.Point.VectorTo(p).AngleTo(this.Direction);
            return Math.Abs(angle) < Constants.Tolerance;
        }

        internal Ray Flip()
        {
            return new Ray(this.Point, this.Direction.Negated());
        }

        internal Point Project(Point p)
        {
            var toPoint = this.Point.VectorTo(p);
            var dotProdcut = toPoint.DotProdcut(this.Direction);
            var projected = this.Point + dotProdcut * this.Direction;
            return projected;
        }

        internal Line? PerpendicularLineTo(Point p)
        {
            if (this.IsPointOn(p))
            {
                return null;
            }

            var startPoint = this.Project(p);
            return new Line(startPoint, p);
        }

        internal Point? FirstIntersectionWith(Rect rectangle)
        {
            var quadrant = rectangle.Contains(this.Point)
                ? this.Direction.Quadrant()
                : this.Direction.Negated().Quadrant();

            switch (quadrant)
            {
                case Quadrant.NegativeXPositiveY:
                    return IntersectionPoint(this, rectangle.LeftLine(), true) ??
                           IntersectionPoint(this, rectangle.BottomLine(), true);
                case Quadrant.PositiveXPositiveY:
                    return IntersectionPoint(this, rectangle.RightLine(), true) ??
                           IntersectionPoint(this, rectangle.BottomLine(), true);
                case Quadrant.PositiveXNegativeY:
                    return IntersectionPoint(this, rectangle.RightLine(), true) ??
                           IntersectionPoint(this, rectangle.TopLine(), true);
                case Quadrant.NegativeXNegativeY:
                    return IntersectionPoint(this, rectangle.LeftLine(), true) ??
                           IntersectionPoint(this, rectangle.TopLine(), true);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal Point? FirstIntersectionWith(Circle circle)
        {
            var perp = this.PerpendicularLineTo(circle.Center);
            if (perp == null)
            {
                return circle.Center - circle.Radius * this.Direction;
            }

            var pl = perp.Value.Length;
            if (pl > circle.Radius)
            {
                return null;
            }

            var tangentLength = Math.Sqrt(circle.Radius * circle.Radius - pl * pl);
            var result = perp.Value.StartPoint - tangentLength * this.Direction;
            return this.IsPointOn(result)
                       ? (Point?)result
                       : null;
        }

        // http://www.mare.ee/indrek/misc/2d.pdf
        internal Point? FirstIntersectionWith(Ellipse ellipse)
        {
            var nx = this.Direction.X;
            var nx2 = nx * nx;
            var ny = this.Direction.Y;
            var ny2 = ny * ny;
            var x0 = ellipse.Center.X;
            var x02 = x0 * x0;
            var y0 = ellipse.Center.Y;
            var y02 = y0 * y0;
            var a = ellipse.RadiusX;
            var a2 = a * a;
            var b = ellipse.RadiusY;
            var b2 = b * b;
            var A = nx2 * b2 + ny2 * a2;
            if (Math.Abs(A) < Constants.Tolerance)
            {
                return null;
            }

            var B = 2 * x0 * nx * b2 + 2 * y0 * ny * a2;
            var C = x02 * b2 + y02 * a2 - a2 * b2;
            var d = B * B - 4 * A * C;
            if (d < 0)
            {
                return null;
            }

            var s = (-B - Math.Sqrt(d)) / 2 * A;
            var result = ellipse.Center + s * this.Direction;
            return this.IsPointOn(result)
                       ? (Point?)result
                       : null;
        }

        // http://geomalgorithms.com/a05-_intersect-1.html#intersect2D_2Segments()
        private static Point? IntersectionPoint(Ray ray, Line l2, bool mustBeBetweenStartAndEnd)
        {
            var u = ray.Direction;
            var v = l2.Direction;
            var w = ray.Point - l2.StartPoint;
            var d = Perp(u, v);
            if (Math.Abs(d) < Constants.Tolerance)
            {
                // parallel lines
                return null;
            }
            var sI = Perp(v, w) / d;
            var p = ray.Point + sI * u;
            if (mustBeBetweenStartAndEnd)
            {
                if (ray.IsPointOn(p) && l2.IsPointOnLine(p))
                {
                    return p;
                }

                return null;
            }

            return p;
        }

        private static double Perp(Vector u, Vector v)
        {
            return u.X * v.Y - u.Y * v.X;
        }
    }
}
