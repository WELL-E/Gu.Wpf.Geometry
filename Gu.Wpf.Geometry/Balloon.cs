namespace Gu.Wpf.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class Balloon : Shape
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(Balloon),
            new FrameworkPropertyMetadata(
                default(CornerRadius),
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnCornerRadiusChanged));

        public static readonly DependencyProperty ConnectorOffsetProperty = DependencyProperty.Register(
            "ConnectorOffset",
            typeof(Vector),
            typeof(Balloon),
            new FrameworkPropertyMetadata(
                default(Vector),
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnConnectorChanged));

        public static readonly DependencyProperty ConnectorAngleProperty = DependencyProperty.Register(
            "ConnectorAngle",
            typeof(double),
            typeof(Balloon),
            new FrameworkPropertyMetadata(
                15.0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnConnectorChanged));

        private readonly PenCache penCache = new PenCache();
        private Geometry balloonGeometry;
        private Geometry boxGeometry;
        private Geometry connectorGeometry;

        static Balloon()
        {
            StretchProperty.OverrideMetadata(typeof(Balloon), new FrameworkPropertyMetadata(Stretch.Fill));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        public Vector ConnectorOffset
        {
            get { return (Vector)this.GetValue(ConnectorOffsetProperty); }
            set { this.SetValue(ConnectorOffsetProperty, value); }
        }

        public double ConnectorAngle
        {
            get { return (double)this.GetValue(ConnectorAngleProperty); }
            set { this.SetValue(ConnectorAngleProperty, value); }
        }

        protected override Geometry DefiningGeometry => this.boxGeometry ?? Geometry.Empty;

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(this.StrokeThickness, this.StrokeThickness);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var pen = this.penCache.GetPen(this.Stroke, this.StrokeThickness);
            drawingContext.DrawGeometry(this.Fill, pen, this.balloonGeometry);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.UpdateCachedGeometries();
            this.InvalidateVisual();
        }

        protected virtual void UpdateCachedGeometries()
        {
            if (this.RenderSize == Size.Empty)
            {
                this.boxGeometry = Geometry.Empty;
                this.connectorGeometry = Geometry.Empty;
                this.balloonGeometry = Geometry.Empty;
                return;
            }

            this.boxGeometry = this.CreateBoxGeometry(this.RenderSize);
            this.connectorGeometry = this.CreateConnectorGeometry(this.RenderSize);
            this.balloonGeometry = this.CreateGeometry(this.boxGeometry, this.connectorGeometry);
        }

        protected virtual Geometry CreateBoxGeometry(Size size)
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

        protected virtual Geometry CreateConnectorGeometry(Size size)
        {
            if (this.ConnectorOffset == default(Vector) || size.IsEmpty)
            {
                return Geometry.Empty;
            }

            var width = size.Width - this.StrokeThickness;
            var height = size.Height - this.StrokeThickness;
            var mp = new Point(width / 2, height / 2);
            var direction = this.ConnectorOffset.Normalized();
            var length = width * width + height * height;
            var rectangle = new Rect(new Point(0, 0), size);
            rectangle.Inflate(-this.StrokeThickness, -this.StrokeThickness);
            if (rectangle.IsEmpty)
            {
                return Geometry.Empty;
            }

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
                     .InflateBy(-this.StrokeThickness/2)
                     .WithMin(0);
        }

        protected virtual Geometry CreateGeometry(Geometry boxGeometry, Geometry connectorGeometry)
        {
            var ballonGeometry = new CombinedGeometry(GeometryCombineMode.Union, boxGeometry, connectorGeometry);
            ballonGeometry.Freeze();
            return ballonGeometry;
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (Balloon)d;
            if (balloon.IsInitialized)
            {
                balloon.UpdateCachedGeometries();
            }
        }

        private static void OnConnectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (Balloon)d;
            if (balloon.IsInitialized)
            {
                balloon.UpdateCachedGeometries();
            }
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
                    if (ip != null)
                    {
                        return ip.Value;
                    }

                    throw new InvalidOperationException("Could not find intersection with radius");
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
                if (line.Direction.X > 0 && line.Direction.Y > 0)
                {
                    var r = cornerRadius.TopLeft;
                    return new Circle(rectangle.TopLeft.WithOffset(r, r), r);
                }
                if (line.Direction.X < 0 && line.Direction.Y > 0)
                {
                    var r = cornerRadius.TopRight;
                    return new Circle(rectangle.TopRight.WithOffset(-r, r), r);
                }
                if (line.Direction.X < 0 && line.Direction.Y < 0)
                {
                    var r = cornerRadius.BottomRight;
                    return new Circle(rectangle.BottomRight.WithOffset(-r, -r), r);
                }
                if (line.Direction.X > 0 && line.Direction.Y < 0)
                {
                    var r = cornerRadius.BottomLeft;
                    return new Circle(rectangle.BottomLeft.WithOffset(r, -r), r);
                }

                throw new InvalidOperationException("Could not find corner radius");
            }
        }

        private class PenCache
        {
            private Brush brush;
            private double strokeThickness;
            private Pen pen;

            public Pen GetPen(Brush brush, double strokeThickness)
            {
                if (this.brush == brush && this.strokeThickness == strokeThickness)
                {
                    return this.pen;
                }

                this.brush = brush;
                this.strokeThickness = strokeThickness;
                this.pen = new Pen(brush, strokeThickness);
                return this.pen;
            }
        }
    }
}