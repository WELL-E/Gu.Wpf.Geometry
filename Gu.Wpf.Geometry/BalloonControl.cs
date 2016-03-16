namespace Gu.Wpf.Geometry
{
    using System.Windows;
    using System.Windows.Controls;

    public class BalloonControl : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(BalloonControl));

        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register(
            "PlacementTarget", 
            typeof (UIElement), 
            typeof (BalloonControl), 
            new PropertyMetadata(default(UIElement)));

        static BalloonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BalloonControl), new FrameworkPropertyMetadata(typeof(BalloonControl)));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }
    }
}