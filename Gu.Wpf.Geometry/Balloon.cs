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
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnCornerRadiusChanged));

        public static readonly DependencyProperty ConnectorPointProperty = DependencyProperty.Register(
            "ConnectorPoint",
            typeof(Point),
            typeof(Balloon),
            new PropertyMetadata(default(Point), OnConnectorChanged));

        public static readonly DependencyProperty ConnectorAngleProperty = DependencyProperty.Register(
            "ConnectorAngle",
            typeof(double),
            typeof(Balloon),
            new PropertyMetadata(default(double), OnConnectorChanged));

        private System.Windows.Media.Geometry balloonGeometry;

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

        protected override Geometry DefiningGeometry => this.balloonGeometry ?? Geometry.Empty;

        protected override Size MeasureOverride(Size constraint)
        {
            this.balloonGeometry = CreateGeometry(constraint);
            return constraint;
        }

        protected virtual Geometry CreateBoxGeometry(Size size)
        {
            var width = size.Width - StrokeThickness;
            var height = size.Height - StrokeThickness;

            var boxGeometry = new StreamGeometry();
            using (var context = boxGeometry.Open())
            {
                var p = this.CornerRadius.TopLeft > 0
                    ? new Point(this.CornerRadius.TopLeft + StrokeThickness / 2, StrokeThickness / 2)
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

            return boxGeometry;
        }

        protected virtual Geometry CreateConnectorGeometry(Size size)
        {
            var width = size.Width - StrokeThickness;
            var height = size.Height - StrokeThickness;
            var connectorGeometry = new StreamGeometry();
            using (var context = connectorGeometry.Open())
            {
                context.BeginFigure(ConnectorPoint, true, true);
                var mp = new Point(width / 2, height / 2);
                var v = mp - ConnectorPoint;
                var p = ConnectorPoint + v.Rotate(ConnectorAngle / 2);
                context.LineTo(p, true, true);
                p = ConnectorPoint + v.Rotate(-ConnectorAngle / 2);
                context.LineTo(p, true, true);
            }

            return connectorGeometry;
        }

        private Geometry CreateGeometry(Size size)
        {
            if (this.balloonGeometry != null)
            {
                return this.balloonGeometry;
            }

            var boxGeometry = CreateBoxGeometry(size);
            var connectorGeometry = CreateConnectorGeometry(size);

            var ballonGeometry = new CombinedGeometry(GeometryCombineMode.Union, boxGeometry, connectorGeometry);
            ballonGeometry.Freeze();
            return ballonGeometry;
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (Balloon)d;
            balloon.balloonGeometry = null;
        }

        private static void OnConnectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloon = (Balloon)d;
            balloon.balloonGeometry = null;
        }
    }
}