namespace Gu.Wpf.Geometry
{
    using System.Windows;
    using System.Windows.Controls;

    public class BalloonControl : ContentControl
    {
        static BalloonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BalloonControl), new FrameworkPropertyMetadata(typeof(BalloonControl)));
        }
    }
}