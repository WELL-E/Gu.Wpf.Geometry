namespace Gu.Wpf.Geometry
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class BalloonControl : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty ConnectorOffsetProperty = Balloon.ConnectorOffsetProperty.AddOwner(typeof(BalloonControl));

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

        public UIElement PlacementTarget
        {
            get { return (UIElement)this.GetValue(PlacementTargetProperty); }
            set { this.SetValue(PlacementTargetProperty, value); }
        }

        public PlacementOptions PlacementOptions
        {
            get { return (PlacementOptions)this.GetValue(PlacementOptionsProperty); }
            set { this.SetValue(PlacementOptionsProperty, value); }
        }

        protected virtual void OnLayoutUpdated(object _, EventArgs __)
        {
            if (this.IsLoaded && this.PlacementTarget != null)
            {
                //var p1 = this.PointToScreen(new Point(0, 0));
                //var placementRect = new Rect(new Point(0, 0), this.PlacementTarget.RenderSize);
                //var p2 = this.PlacementOptions?.GetPoint(placementRect) ?? new Point(0, 0);
                //p2 = this.PlacementTarget.PointToScreen(p2);
                //var v = p2 - p1;
                //if (this.PlacementOptions != null && this.PlacementOptions.Offset != 0)
                //{
                //    var uv = v.Normalized();
                //    var offset = Vector.Multiply(this.PlacementOptions.Offset, uv);
                //    v = v + offset;
                //}
                //this.SetCurrentValue(ConnectorOffsetProperty, new Point(v.X, v.Y));
            }
            else
            {
                this.InvalidateProperty(ConnectorOffsetProperty);
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
            WeakEventManager<UIElement, EventArgs>.RemoveHandler((UIElement)e.OldValue, nameof(LayoutUpdated), balloonControl.OnLayoutUpdated);
            WeakEventManager<UIElement, EventArgs>.AddHandler((UIElement)e.NewValue, nameof(LayoutUpdated), balloonControl.OnLayoutUpdated);
        }
    }
}