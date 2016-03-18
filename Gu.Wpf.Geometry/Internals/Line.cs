namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    [DebuggerDisplay("{StartPoint.ToString()} -> {EndPoint.ToString()}")]
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

        public Line RotateAroundStartPoint(double angleInDegrees)
        {
            var v = this.EndPoint - this.StartPoint;
            v = v.Rotate(angleInDegrees);
            var ep = this.StartPoint + v;
            return new Line(this.StartPoint, ep);
        }

        public Vector PerpendicularDirection
        {
            get
            {
                var direction = this.Direction;
                return new Vector(direction.Y, -direction.X);
            }
        }

        public override string ToString()
        {
            return this.ToString(string.Empty);
        }

        public string ToString(string format)
        {
            return $"{this.StartPoint.ToString(format)}; {this.EndPoint.ToString(format)}";
        }

        internal static Line Parse(string text)
        {
            var strings = text.Split(';');
            if (strings.Length != 2)
            {
                throw new ArgumentException();
            }

            var sp = Point.Parse(strings[0]);
            var ep = Point.Parse(strings[1]);
            return new Line(sp, ep);
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
            if (this.StartPoint.DistanceTo(p) < 1E-3)
            {
                return true;
            }

            var v = p - this.StartPoint;
            var angleBetween = Vector.AngleBetween(this.Direction, v);
            if (Math.Abs(angleBetween) > 1E-3)
            {
                return false;
            }

            return v.Length <= this.Length + 1E-3;
        }

        internal Line? TrimOrExtendEndWith(Line other)
        {
            if (this.EndPoint.DistanceTo(other.StartPoint) < 1e-3)
            {
                return this;
            }

            var ip = IntersectionPoint(this, other, false);
            if (ip == null)
            {
                return null;
            }

            return new Line(this.StartPoint, ip.Value);
        }

        internal Line? TrimOrExtendStartWith(Line other)
        {
            if (this.StartPoint.DistanceTo(other.EndPoint) < 1e-3)
            {
                return this;
            }

            var ip = IntersectionPoint(this, other, false);
            if (ip == null)
            {
                return null;
            }

            return new Line(ip.Value, this.EndPoint);
        }

        internal Point? IntersectWith(Line other, bool mustBeBetweenStartAndEnd = true)
        {
            return IntersectionPoint(this, other, mustBeBetweenStartAndEnd);
        }

        internal Point? ClosestIntersection(Rect rectangle)
        {
            Point ip;
            if (rectangle.Contains(this.StartPoint))
            {
                if (rectangle.TopLine().TryFindIntersectionPoint(this, out ip) ||
                    rectangle.RightLine().TryFindIntersectionPoint(this, out ip) ||
                    rectangle.BottomLine().TryFindIntersectionPoint(this, out ip) ||
                    rectangle.LeftLine().TryFindIntersectionPoint(this, out ip))
                {
                    return ip;
                }

                return null;
            }

            if (this.StartPoint.X < 0)
            {
                if (rectangle.LeftLine().TryFindIntersectionPoint(this, out ip))
                {
                    return ip;
                }
            }

            if (this.StartPoint.X > rectangle.Width)
            {
                if (rectangle.RightLine().TryFindIntersectionPoint(this, out ip))
                {
                    return ip;
                }
            }

            if (this.StartPoint.Y < 0)
            {
                if (rectangle.TopLine().TryFindIntersectionPoint(this, out ip))
                {
                    return ip;
                }

                return null;
            }

            if (this.StartPoint.Y > rectangle.Height)
            {
                if (rectangle.BottomLine().TryFindIntersectionPoint(this, out ip))
                {
                    return ip;
                }

                return null;
            }

            return null;
        }


        internal Line PerpendicularLineTo(Point p)
        {
            var toPoint = this.StartPoint.VectorTo(p);
            var dotProdcut = toPoint.DotProdcut(this.Direction);
            var startPoint = this.StartPoint + dotProdcut*this.Direction;
            return new Line(startPoint, p);
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
            if (mustBeBetweenStartAndEnd)
            {
                if (l1.IsPointOnLine(p) && l2.IsPointOnLine(p))
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
