namespace Gu.Wpf.Geometry
{
    internal static class LineExt
    {
        internal static string ToString(this Line? l, string format = "F1")
        {
            if (l == null)
            {
                return "null";
            }

            return l.Value.ToString(format);
        }
    }
}