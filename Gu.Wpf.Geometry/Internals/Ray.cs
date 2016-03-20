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
            return perp.Value.StartPoint - tangentLength * this.Direction;
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
