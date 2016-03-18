namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;

    public class BoxBalloon : Balloon
    {
        protected override void UpdateConnectorOffset()
        {
            if (this.PlacementTarget != null)
            {
                if (this.IsVisible && this.PlacementTarget.IsVisible)
                {
                    var rect = new Rect(new Point(0, 0), this.RenderSize).ToScreen(this);
                    var placementRect = new Rect(new Point(0, 0), this.PlacementTarget.RenderSize);
                    var tp = this.PlacementOptions?.GetPoint(placementRect) ?? new Point(0, 0);
                    tp = this.PlacementTarget.PointToScreen(tp);
                    if (rect.Contains(tp))
                    {
                        this.SetCurrentValue(ConnectorOffsetProperty, new Vector(0, 0));
                        return;
                    }

                    var mp = rect.MidPoint();
                    var ip = new Line(mp, tp).ClosestIntersection(rect);
                    if (ip == null)
                    {
                        throw new InvalidOperationException("bug in the library");
                    }

                    var v = tp - ip.Value;
                    if (this.PlacementOptions != null && this.PlacementOptions.Offset != 0)
                    {
                        var uv = v.Normalized();
                        var offset = Vector.Multiply(this.PlacementOptions.Offset, uv);
                        v = v + offset;
                    }

                    this.SetCurrentValue(ConnectorOffsetProperty, v);
                }
            }
            else
            {
                this.InvalidateProperty(ConnectorOffsetProperty);
            }
        }

        protected override Geometry CreateBoxGeometry(Size size)
        {
            var width = size.Width - this.StrokeThickness;
            var height = size.Height - this.StrokeThickness;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                var cr = this.AdjustedCornerRadius();
                var p = cr.TopLeft > 0
                    ? new Point(cr.TopLeft + this.StrokeThickness / 2, this.StrokeThickness / 2)
                    : new Point(this.StrokeThickness / 2, this.StrokeThickness / 2);
                context.BeginFigure(p, true, true);
                p = p.WithOffset(width - cr.TopLeft - cr.TopRight, 0);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, cr.TopRight, cr.TopRight);

                p = p.WithOffset(0, height - cr.TopRight - cr.BottomRight);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, -cr.BottomRight, cr.BottomRight);

                p = p.WithOffset(-width + cr.BottomRight + cr.BottomLeft, 0);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, -cr.BottomLeft, -cr.BottomLeft);

                p = p.WithOffset(0, -height + cr.TopLeft + cr.BottomLeft);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, cr.TopLeft, -cr.TopLeft);
            }

            geometry.Freeze();
            return geometry;
        }

        protected override Geometry CreateConnectorGeometry(Size size)
        {
            if (this.ConnectorOffset == default(Vector) || size.IsEmpty)
            {
                return Geometry.Empty;
            }

            var rectangle = new Rect(new Point(0, 0), size);
            rectangle.Inflate(-this.StrokeThickness, -this.StrokeThickness);
            if (rectangle.IsEmpty)
            {
                return Geometry.Empty;
            }

            var width = size.Width - this.StrokeThickness;
            var height = size.Height - this.StrokeThickness;
            var mp = new Point(width / 2, height / 2);
            var direction = this.ConnectorOffset.Normalized();
            var length = width * width + height * height;
            var line = new Line(mp, mp + length * direction);

            var ip = line.ClosestIntersection(rectangle);
            if (ip == null)
            {
                Debug.Assert(false, $"Line {line} does not intersect rectangle {rectangle}");
                return Geometry.Empty;
            }

            var cr = this.AdjustedCornerRadius();
            var sp = ip.Value + this.ConnectorOffset;
            line = new Line(sp, mp + length * direction.Negated());
            var p1 = ConnectorPoint.Find(line, this.ConnectorAngle / 2, rectangle, cr);
            var p2 = ConnectorPoint.Find(line, -this.ConnectorAngle / 2, rectangle, cr);

            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(sp, true, true);
                context.LineTo(p1, true, true);
                context.LineTo(p2, true, true);
            }

            geometry.Freeze();
            return geometry;
        }

        protected virtual CornerRadius AdjustedCornerRadius()
        {
            var cr = this.CornerRadius.InflateBy(-this.StrokeThickness / 2)
                                 .WithMin(0);
            var left = cr.TopLeft + cr.BottomLeft;
            var right = cr.TopRight + cr.BottomRight;
            var top = cr.TopLeft + cr.TopRight;
            var bottom = cr.BottomLeft + cr.BottomRight;
            if (left < this.ActualHeight &&
                right < this.ActualHeight &&
                top < this.ActualWidth &&
                bottom < this.ActualWidth)
            {
                return cr;
            }

            var factor = Math.Min(Math.Min(this.ActualWidth / top, this.ActualWidth / bottom),
                                  Math.Min(this.ActualHeight / left, this.ActualHeight / right));
            return cr.ScaleBy(factor)
                     .InflateBy(-this.StrokeThickness / 2)
                     .WithMin(0);
        }


        private static class ConnectorPoint
        {
            internal static Point Find(Line line, double angle, Rect rectangle, CornerRadius cornerRadius)
            {
                var rotated = line.RotateAroundStartPoint(angle);
                return FindForRotated(rotated, rectangle, cornerRadius);
            }

            private static Point FindForRotated(Line line, Rect rectangle, CornerRadius cornerRadius)
            {
                var ip = line.ClosestIntersection(rectangle);
                if (ip == null)
                {
                    var radius = FindClosestCornerRadius(line, rectangle, cornerRadius);
                    return FindTangentPoint(line, radius);
                }

                if (IsOnCornerRadius(ip.Value, rectangle, cornerRadius))
                {
                    var radius = FindClosestCornerRadius(line, rectangle, cornerRadius);
                    ip = radius.ClosestIntersection(line);
                    if (ip == null)
                    {
                        return FindTangentPoint(line, radius);
                    }

                    return ip.Value;
                }

                return ip.Value;
            }

            private static bool IsOnCornerRadius(Point intersectionPoint, Rect rectangle, CornerRadius cornerRadius)
            {
                if (intersectionPoint.DistanceTo(rectangle.TopLeft) < cornerRadius.TopLeft)
                {
                    return true;
                }

                if (intersectionPoint.DistanceTo(rectangle.TopRight) < cornerRadius.TopRight)
                {
                    return true;
                }

                if (intersectionPoint.DistanceTo(rectangle.BottomRight) < cornerRadius.BottomRight)
                {
                    return true;
                }

                if (intersectionPoint.DistanceTo(rectangle.BottomLeft) < cornerRadius.BottomLeft)
                {
                    return true;
                }

                return false;
            }

            private static Point FindTangentPoint(Line line, Circle radius)
            {
                var toCenter = line.StartPoint.VectorTo(radius.Center);
                if (radius.Radius == 0)
                {
                    return line.StartPoint + toCenter;
                }

                if (line.Direction.AngleTo(toCenter) > 0)
                {
                    var perp = radius.Radius * toCenter.Rotate(90).Normalized();
                    return line.StartPoint + toCenter + perp;
                }
                else
                {
                    var perp = radius.Radius * toCenter.Rotate(-90).Normalized();
                    return line.StartPoint + toCenter + perp;
                }
            }

            private static Circle FindClosestCornerRadius(Line line, Rect rectangle, CornerRadius cornerRadius)
            {
                if (line.DistanceTo(rectangle.TopLeft) <= cornerRadius.TopLeft)
                {
                    var r = cornerRadius.TopLeft;
                    return new Circle(rectangle.TopLeft.WithOffset(r, r), r);
                }
                if (line.DistanceTo(rectangle.TopRight) <= cornerRadius.TopRight)
                {
                    var r = cornerRadius.TopRight;
                    return new Circle(rectangle.TopRight.WithOffset(-r, r), r);
                }
                if (line.DistanceTo(rectangle.BottomRight) <= cornerRadius.BottomRight)
                {
                    var r = cornerRadius.BottomRight;
                    return new Circle(rectangle.BottomRight.WithOffset(-r, -r), r);
                }
                if (line.DistanceTo(rectangle.BottomLeft) <= cornerRadius.BottomLeft)
                {
                    var r = cornerRadius.BottomLeft;
                    return new Circle(rectangle.BottomLeft.WithOffset(r, -r), r);
                }

                throw new InvalidOperationException("Could not find corner radius");
            }
        }
    }
}