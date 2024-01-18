namespace Disk.Data.Impl
{
    internal class Point2D<CoordType> where CoordType : IConvertible, new()
    {
        public CoordType X { get; set; }

        public CoordType Y { get; set; }

        public Point2D(CoordType x, CoordType y)
        {
            X = x;
            Y = y;
        }

        public Point2D()
        {
            X = new();
            Y = new();
        }

        public static double GetDistance(Point2D<CoordType> p, IFormatProvider formatProvider)
            => Math.Sqrt(
                Math.Pow(p.X.ToDouble(formatProvider), 2) + 
                Math.Pow(p.Y.ToDouble(formatProvider), 2)
                );

        public override string ToString() => $"{X};{Y}";
    }
}
