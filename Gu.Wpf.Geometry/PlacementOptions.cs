namespace Gu.Wpf.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using Gu.Wpf.Geometry;

    [TypeConverter(typeof(PlacementOptionsConverter))]
    public class PlacementOptions
    {
        public VerticalPlacement VerticalPlacement { get; set; }

        public HorizontalPlacement HorizontalPlacement { get; set; }

        public double Offset { get; set; }

        public Point GetPoint(Rect rect)
        {
            switch (this.VerticalPlacement)
            {
                case VerticalPlacement.Top:
                    switch (HorizontalPlacement)
                    {
                        case HorizontalPlacement.Left:
                            return rect.TopLeft;
                        case HorizontalPlacement.Center:
                            return MidPoint(rect.TopLeft, rect.TopRight);
                        case HorizontalPlacement.Right:
                            return rect.TopRight;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case VerticalPlacement.Center:
                    switch (HorizontalPlacement)
                    {
                        case HorizontalPlacement.Left:
                            return MidPoint(rect.BottomLeft, rect.TopLeft);
                        case HorizontalPlacement.Center:
                            return MidPoint(rect.TopLeft, rect.BottomRight);
                        case HorizontalPlacement.Right:
                            return MidPoint(rect.BottomRight, rect.TopRight);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case VerticalPlacement.Bottom:
                    switch (HorizontalPlacement)
                    {
                        case HorizontalPlacement.Left:
                            return rect.BottomLeft;
                        case HorizontalPlacement.Center:
                            return MidPoint(rect.BottomLeft, rect.BottomRight);
                        case HorizontalPlacement.Right:
                            return rect.BottomRight;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static Point MidPoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }
    }
}
