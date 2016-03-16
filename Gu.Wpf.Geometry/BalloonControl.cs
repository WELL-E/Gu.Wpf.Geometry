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

        protected virtual void UpdateConnectorPoint(object _, EventArgs __)
        {
            if (this.IsLoaded && PlacementTarget != null)
            {
                var p1 = this.PointToScreen(new Point(0, 0));
                var p2 = PlacementTarget.PointToScreen(new Point(0, 0));
                var v = p2 - p1;
                SetCurrentValue(ConnectorPointProperty, new Point(v.X, v.Y));
            }
            else
            {
                InvalidateProperty(ConnectorPointProperty);
            }
        }

        private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloonControl = (BalloonControl)d;
            balloonControl.UpdateConnectorPoint(null, null);
            balloonControl.LayoutUpdated -= balloonControl.UpdateConnectorPoint;
            balloonControl.LayoutUpdated += balloonControl.UpdateConnectorPoint;
            WeakEventManager<UIElement, EventArgs>.RemoveHandler((UIElement)e.OldValue, nameof(UIElement.LayoutUpdated), balloonControl.UpdateConnectorPoint);
            WeakEventManager<UIElement, EventArgs>.AddHandler((UIElement)e.NewValue, nameof(UIElement.LayoutUpdated), balloonControl.UpdateConnectorPoint);
        }
    }
}