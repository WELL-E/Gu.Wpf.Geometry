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
            EventManager.RegisterClassHandler(typeof(BalloonControl), LoadedEvent, new RoutedEventHandler(OnLoaded));
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

        protected virtual void UpdateConnectorOffset()
        {
            if (this.IsLoaded && this.IsVisible && this.PlacementTarget != null)
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
                var ip = new Line(mp, tp).IntersectWith(rect);
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
            else if (this.PlacementTarget == null)
            {
                this.InvalidateProperty(ConnectorOffsetProperty);
            }
        }

        protected virtual void OnLayoutUpdated(object _, EventArgs __)
        {
            this.UpdateConnectorOffset();
        }

        protected virtual void OnLoaded()
        {
            UpdateConnectorOffset();
        }

        private static void OnPlacementOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloonControl = (BalloonControl)d;
            balloonControl.OnLayoutUpdated(null, null);
        }

        private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloonControl = (BalloonControl)d;
            balloonControl.UpdateConnectorOffset();
            // unsubscribing and subscribing here to have only one subscription
            balloonControl.LayoutUpdated -= balloonControl.OnLayoutUpdated;
            balloonControl.LayoutUpdated += balloonControl.OnLayoutUpdated;
            WeakEventManager<UIElement, EventArgs>.RemoveHandler((UIElement)e.OldValue, nameof(LayoutUpdated), balloonControl.OnLayoutUpdated);
            WeakEventManager<UIElement, EventArgs>.AddHandler((UIElement)e.NewValue, nameof(LayoutUpdated), balloonControl.OnLayoutUpdated);
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((BalloonControl)sender).OnLoaded();
        }
    }
}