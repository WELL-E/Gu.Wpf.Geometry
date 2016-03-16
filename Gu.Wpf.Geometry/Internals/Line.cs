namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    [DebuggerDisplay("StartPoint: {StartPoint.ToDebugString()} EndPoint: {EndPoint.ToDebugString()}")]
    internal struct Line
    {
        public readonly Point StartPoint;
        public readonly Point EndPoint;

        public Line(Point startPoint, Point endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        public Point MidPoint
        {
            get
            {
                var x = (this.StartPoint.X + this.EndPoint.X) / 2;
                var y = (this.StartPoint.Y + this.EndPoint.Y) / 2;
                return new Point(x, y);
            }
        }

        public double Length => (this.EndPoint - this.StartPoint).Length;

        public Vector Direction
        {
            get
            {
                var v = this.EndPoint - this.StartPoint;
                v.Normalize();
                return v;
            }
        }

        public Vector PerpendicularDirection
        {
            get
            {
                var direction = this.Direction;
                return new Vector(direction.Y, -direction.X);
            }
        }

        public string ToString(string format)
        {
            return $"{this.StartPoint.ToDebugString(format)}; {this.EndPoint.ToDebugString(format)}";
        }

        internal Line Offset(double distance)
        {
            var v = this.PerpendicularDirection;
            var sp = this.StartPoint.WithOffset(v, distance);
            var ep = this.EndPoint.WithOffset(v, distance);
            return new Line(sp, ep);
        }

        internal bool IsPointOnLine(Point p)
        {
            var v = this.StartPoint - p;
            if (Math.Abs(Vector.AngleBetween(this.Direction, v)) > 1E-3)
            {
                return false;
            }

            return v.Length < this.Length;
        }

        internal Line TrimOrExtendEndWith(Line other)
        {
            if (this.EndPoint.DistanceTo(other.StartPoint) < 1e-3)
            {
                return this;
            }
            var ip = IntersectionPoint(this, other, false);
            return new Line(this.StartPoint, ip.Value);
        }

        internal Line TrimOrExtendStartWith(Line other)
        {
            if (this.StartPoint.DistanceTo(other.EndPoint) < 1e-3)
            {
                return this;
            }
            var ip = IntersectionPoint(this, other, false);
            return new Line(ip.Value, this.EndPoint);
        }

        internal Point? IntersectWith(Line other, bool mustBeBetweenStartAndEnd = true)
        {
            return IntersectionPoint(this, other, mustBeBetweenStartAndEnd);
        }

        internal Point? IntersectWith(Rect rectangle)
        {
            Point ip;
            if (this.TryFindIntersectionPoint(new Line(rectangle.TopLeft, rectangle.TopRight), out ip) ||
               this.TryFindIntersectionPoint(new Line(rectangle.TopRight, rectangle.BottomRight), out ip) ||
               this.TryFindIntersectionPoint(new Line(rectangle.BottomRight, rectangle.BottomLeft), out ip) ||
               this.TryFindIntersectionPoint(new Line(rectangle.BottomLeft, rectangle.TopLeft), out ip))
            {
                return ip;
            }

            return null;
        }

        internal bool TryFindIntersectionPoint(Line other, out Point intersectionPoint)
        {
            var ip = IntersectionPoint(this, other, true);
            if (ip == null)
            {
                intersectionPoint = default(Point);
                return false;
            }

            intersectionPoint = ip.Value;
            return true;
        }

        /// <summary>
        /// http://geomalgorithms.com/a05-_intersect-1.html#intersect2D_2Segments()
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        private static Point? IntersectionPoint(Line l1, Line l2, bool mustBeBetweenStartAndEnd = true)
        {
            var u = l1.Direction;
            var v = l2.Direction;
            var w = l1.StartPoint - l2.StartPoint;
            var d = Perp(u, v);
            var sI = Perp(v, w) / d;
            var p = l1.StartPoint + sI * u;
            if (mustBeBetweenStartAndEnd && !l1.IsPointOnLine(p))
            {
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
