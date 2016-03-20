namespace Gu.Wpf.Geometry
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    public class EllipseBalloon : BalloonBase
    {
        private static readonly DependencyProperty EllipseProperty = DependencyProperty.Register(
            "Ellipse",
            typeof(Ellipse),
            typeof(EllipseBalloon),
            new PropertyMetadata(default(Ellipse)));

        protected override Geometry GetOrCreateBoxGeometry(Size renderSize)
        {
            var width = renderSize.Width - this.StrokeThickness;
            var height = renderSize.Height - this.StrokeThickness;
            var rx = width / 2;
            var ry = height / 2;
            this.SetValue(EllipseProperty, new Ellipse(new Point(rx, ry), rx, ry));
            if (width <= 0 || height <= 0)
            {
                return Geometry.Empty;
            }

            if (this.BoxGeometry is EllipseGeometry)
            {
                return this.BoxGeometry;
            }

            var geometry = new EllipseGeometry();
            geometry.Bind(EllipseGeometry.CenterProperty)
                    .OneWayTo(this, EllipseProperty, EllipseCenterConverter.Default);
            geometry.Bind(EllipseGeometry.RadiusXProperty)
                    .OneWayTo(this, EllipseProperty, EllipseRadiusXConverter.Default);
            geometry.Bind(EllipseGeometry.RadiusYProperty)
                    .OneWayTo(this, EllipseProperty, EllipseRadiusYConverter.Default);
            return geometry;
        }

        protected override Geometry GetOrCreateConnectorGeometry(Size renderSize)
        {
            var width = renderSize.Width - this.StrokeThickness;
            var height = renderSize.Height - this.StrokeThickness;
            var rx = width / 2;
            var ry = height / 2;
            var ellipse = new Ellipse(new Point(rx, ry), rx, ry);
            if (ellipse.IsZero)
            {
                return Geometry.Empty;
            }

            var direction = this.ConnectorOffset;
            var ip = ellipse.PointOnCircumference(direction);
            var vertexPoint = ip + this.ConnectorOffset;
            var ray = new Ray(vertexPoint, this.ConnectorOffset.Negated());

            var p1 = ConnectorPoint.Find(ray, this.ConnectorAngle / 2, ellipse);
            var p2 = ConnectorPoint.Find(ray, -this.ConnectorAngle / 2, ellipse);

            this.SetValue(ConnectorVertexPointProperty, vertexPoint);
            this.SetValue(ConnectorPoint1Property, p1);
            this.SetValue(ConnectorPoint2Property, p2);
            if (this.ConnectorGeometry is PathGeometry)
            {
                return this.ConnectorGeometry;
            }

            var figure = this.CreatePathFigureStartingAt(ConnectorPoint1Property);
            figure.Segments.Add(this.CreateLineSegmentTo(ConnectorVertexPointProperty));
            figure.Segments.Add(this.CreateLineSegmentTo(ConnectorPoint2Property));
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }

        private PathFigure CreatePathFigureStartingAt(DependencyProperty property)
        {
            var figure = new PathFigure { IsClosed = true };
            figure.Bind(PathFigure.StartPointProperty)
                .OneWayTo(this, property);
            return figure;
        }

        private LineSegment CreateLineSegmentTo(DependencyProperty property)
        {
            var lineSegment = new LineSegment { IsStroked = true };
            lineSegment.Bind(LineSegment.PointProperty)
                .OneWayTo(this, property);
            return lineSegment;
        }

        private static class ConnectorPoint
        {
            internal static Point Find(Ray toCenter, double angle, Ellipse ellipse)
            {
                var ip = toCenter.Rotate(angle)
                                 .FirstIntersectionWith(ellipse);
                if (ip != null)
                {
                    return ip.Value;
                }

                return FindTangentPoint(toCenter, Math.Sign(angle), ellipse);
            }

            private static Point FindTangentPoint(Ray toCenter, int sign, Ellipse ellipse)
            {
                var angle = sign > 0 ? 90 : -90;
                var rotate = toCenter.Direction.Rotate(angle);
                return ellipse.PointOnCircumference(rotate);
            }
        }

        private class EllipseCenterConverter : IValueConverter
        {
            internal static readonly EllipseCenterConverter Default = new EllipseCenterConverter();

            private EllipseCenterConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                return ((Ellipse)value).Center;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }

        private class EllipseRadiusXConverter : IValueConverter
        {
            internal static readonly EllipseRadiusXConverter Default = new EllipseRadiusXConverter();

            private EllipseRadiusXConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                return ((Ellipse)value).RadiusX;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }

        private class EllipseRadiusYConverter : IValueConverter
        {
            internal static readonly EllipseRadiusYConverter Default = new EllipseRadiusYConverter();

            private EllipseRadiusYConverter()
            {
            }

            public object Convert(object value, Type _, object __, CultureInfo ___)
            {
                return ((Ellipse)value).RadiusY;
            }

            public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            {
                throw new NotSupportedException();
            }
        }
    }
}