namespace Gu.Wpf.Geometry
{
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

        public static readonly DependencyProperty ConnectorPointProperty = DependencyProperty.Register(
            "ConnectorPoint",
            typeof(Point),
            typeof(Balloon),
            new FrameworkPropertyMetadata(
                default(Point),
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
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Point ConnectorPoint
        {
            get { return (Point)GetValue(ConnectorPointProperty); }
            set { SetValue(ConnectorPointProperty, value); }
        }

        public double ConnectorAngle
        {
            get { return (double)GetValue(ConnectorAngleProperty); }
            set { SetValue(ConnectorAngleProperty, value); }
        }

        protected override Geometry DefiningGeometry => this.boxGeometry ?? Geometry.Empty;

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(StrokeThickness, StrokeThickness);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var pen = this.penCache.GetPen(this.Stroke, this.StrokeThickness);
            drawingContext.DrawGeometry(Fill, pen, this.balloonGeometry);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateCachedGeometries();
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

            this.boxGeometry = CreateBoxGeometry(this.RenderSize);
            this.connectorGeometry = CreateConnectorGeometry(this.RenderSize, this.boxGeometry);
            this.balloonGeometry = CreateGeometry(this.boxGeometry, this.connectorGeometry);
        }

        protected virtual Geometry CreateBoxGeometry(Size size)
        {
            var width = size.Width - StrokeThickness;
            var height = size.Height - StrokeThickness;

            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                var p = CornerRadius.TopLeft > 0
                    ? new Point(CornerRadius.TopLeft + StrokeThickness / 2, StrokeThickness / 2)
                    : new Point(StrokeThickness / 2, StrokeThickness / 2);
                context.BeginFigure(p, true, true);
                p = p.WithOffset(width - CornerRadius.TopLeft - CornerRadius.TopRight, 0);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, CornerRadius.TopRight, CornerRadius.TopRight);

                p = p.WithOffset(0, height - CornerRadius.TopRight - CornerRadius.BottomRight);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, -CornerRadius.BottomRight, CornerRadius.BottomRight);

                p = p.WithOffset(-width + CornerRadius.BottomRight + CornerRadius.BottomLeft, 0);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, -CornerRadius.BottomLeft, -CornerRadius.BottomLeft);

                p = p.WithOffset(0, -height + CornerRadius.TopLeft + CornerRadius.BottomLeft);
                context.LineTo(p, true, true);
                p = context.DrawCorner(p, CornerRadius.TopLeft, -CornerRadius.TopLeft);
            }
            geometry.Freeze();
            return geometry;
        }

        protected virtual Geometry CreateConnectorGeometry(Size size, Geometry box)
        {
            if (ConnectorPoint == default(Point))
            {
                return Geometry.Empty;
            }

            var width = size.Width - StrokeThickness;
            var height = size.Height - StrokeThickness;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(ConnectorPoint, true, true);
                var mp = new Point(width / 2, height / 2);
                var v = mp - ConnectorPoint;
                var p = ConnectorPoint + v.Rotate(ConnectorAngle / 2);
                context.LineTo(p, true, true);
                p = ConnectorPoint + v.Rotate(-ConnectorAngle / 2);
                context.LineTo(p, true, true);
            }
            geometry.Freeze();
            return geometry;
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
                return pen;
            }
        }
    }
}