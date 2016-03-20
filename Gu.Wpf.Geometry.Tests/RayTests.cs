namespace Gu.Wpf.Geometry.Tests
{
    using System.Windows;

    using Xunit;

    public class RayTests
    {
        [Theory]
        [InlineData("-2,0; 1,0", "0,0; 1", "-1,0")]
        [InlineData("2,0; -1,0", "0,0; 1", "1,0")]
        [InlineData("0,2; 0,-1", "0,0; 1", "0,1")]
        [InlineData("0,-2; 0,1", "0,0; 1", "0,-1")]
        [InlineData("-2,0; -1,0", "0,0; 1", "null")]
        public void FirstIntersectionWithCircle(string ls, string cs, string eps)
        {
            var ray = Ray.Parse(ls);
            var circle = Circle.Parse(cs);
            var expected = eps == "null" ? (Point?)null : Point.Parse(eps);
            var actual = ray.FirstIntersectionWith(circle);
            Assert.Equal(expected, actual, NullablePointComparer.Default);
        }

        [Theory]
        [InlineData("-2,0; 1,0", "0,0; 1; 1", "-1,0")]
        [InlineData("2,0; -1,0", "0,0; 1; 1", "1,0")]
        [InlineData("0,2; 0,-1", "0,0; 1; 1", "0,1")]
        [InlineData("0,-2; 0,1", "0,0; 1; 1", "0,-1")]
        [InlineData("-2,0; -1,0", "0,0; 1; 1", "null")]
        public void FirstIntersectionWithEllipse(string ls, string es, string eps)
        {
            var ray = Ray.Parse(ls);
            var ellipse = Ellipse.Parse(es);
            var expected = eps == "null" ? (Point?)null : Point.Parse(eps);
            var actual = ray.FirstIntersectionWith(ellipse);
            Assert.Equal(expected, actual, NullablePointComparer.Default);
        }
    }
}
