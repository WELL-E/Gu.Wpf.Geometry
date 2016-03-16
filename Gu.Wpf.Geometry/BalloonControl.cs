namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class BalloonControl : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty ConnectorPointProperty = Balloon.ConnectorPointProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty ConnectorAngleProperty = Balloon.ConnectorAngleProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register(
            "PlacementTarget",
            typeof(UIElement),
            typeof(BalloonControl),
            new PropertyMetadata(default(UIElement), OnPlacementTargetChanged));

        public static readonly DependencyProperty PlacementOptionsProperty = DependencyProperty.Register(
            "PlacementOptions",
            typeof(PlacementOptions),
            typeof(BalloonControl),
            new PropertyMetadata(default(PlacementOptions), OnPlacementOptionsChanged));

        static BalloonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BalloonControl), new FrameworkPropertyMetadata(typeof(BalloonControl)));
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

        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        public PlacementOptions PlacementOptions
        {
            get { return (PlacementOptions)GetValue(PlacementOptionsProperty); }
            set { SetValue(PlacementOptionsProperty, value); }
        }

        protected virtual void OnLayoutUpdated(object _, EventArgs __)
        {
            if (this.IsLoaded && PlacementTarget != null)
            {
                var p1 = this.PointToScreen(new Point(0,0));
                var placementRect = new Rect(new Point(0, 0), PlacementTarget.RenderSize);
                var p2 = this.PlacementOptions?.GetPoint(placementRect) ?? new Point(0, 0);
                p2 = PlacementTarget.PointToScreen(p2);
                var v = p2 - p1;
                if (PlacementOptions != null && PlacementOptions.Offset != 0)
                {
                    var uv = v.Normalized();
                    var offset = Vector.Multiply(this.PlacementOptions.Offset, uv);
                    v = v + offset;
                }
                SetCurrentValue(ConnectorPointProperty, new Point(v.X, v.Y));
            }
            else
            {
                InvalidateProperty(ConnectorPointProperty);
            }
        }

        private static void OnPlacementOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloonControl = (BalloonControl)d;
            balloonControl.OnLayoutUpdated(null, null);
        }

        private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloonControl = (BalloonControl)d;
            balloonControl.OnLayoutUpdated(null, null);
            // unsubscribing and subscribing here to have only one subscription
            balloonControl.LayoutUpdated -= balloonControl.OnLayoutUpdated;
            balloonControl.LayoutUpdated += balloonControl.OnLayoutUpdated;
            WeakEventManager<UIElement, EventArgs>.RemoveHandler((UIElement)e.OldValue, nameof(UIElement.LayoutUpdated), balloonControl.OnLayoutUpdated);
            WeakEventManager<UIElement, EventArgs>.AddHandler((UIElement)e.NewValue, nameof(UIElement.LayoutUpdated), balloonControl.OnLayoutUpdated);
        }
    }
}