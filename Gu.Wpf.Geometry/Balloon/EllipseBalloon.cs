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
            var tp = ellipse.PointOnCircumference(direction);
            var vertexPoint = tp + this.ConnectorOffset;
            this.SetValue(ConnectorVertexPointProperty, vertexPoint);
            return Geometry.Empty;
            //var p = ellipse.Point
            //var length = 2 * Math.Max(ellipse.RadiusX, ellipse.RadiusX);
            //var line = ellipse.Center.LineTo(ellipse.Center + length * direction);

            //var ip = line.ClosestIntersection(rectangle);
            //if (ip == null)
            //{
            //    Debug.Assert(false, $"Line {line} does not intersect rectangle {rectangle}");
            //    // ReSharper disable once HeuristicUnreachableCode
            //    return Geometry.Empty;
            //}

            //var cr = this.AdjustedCornerRadius();
            //var sp = ip.Value + this.ConnectorOffset;
            //line = new Line(sp, mp + length * direction.Negated());
            //var p1 = ConnectorPoint.Find(line, this.ConnectorAngle / 2, rectangle, cr);
            //var p2 = ConnectorPoint.Find(line, -this.ConnectorAngle / 2, rectangle, cr);

            //var geometry = new StreamGeometry();
            //using (var context = geometry.Open())
            //{
            //    context.BeginFigure(sp, true, true);
            //    context.LineTo(p1, true, true);
            //    context.LineTo(p2, true, true);
            //}

            //geometry.Freeze();
            //return geometry;
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