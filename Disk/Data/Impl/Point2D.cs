namespace Disk.Data.Impl
{
    class Point2D<CoordType> : IEquatable<Point2D<CoordType>> where CoordType : IConvertible, new()
    {
        public CoordType X { get; set; }
        public CoordType Y { get; set; }

        public double XDbl
        {
            get => X.ToDouble(FormatProvider);
        }

        public double YDbl
        {
            get => Y.ToDouble(FormatProvider);
        }

        protected readonly IFormatProvider? FormatProvider;

        public Point2D(CoordType x, CoordType y, IFormatProvider? formatProvider = null)
        {
            X = x;
            Y = y;
            FormatProvider = formatProvider;
        }

        public Point2D()
        {
            X = new();
            Y = new();
            FormatProvider = null;
        }

        public double GetDistance(Point2D<CoordType> p)
            => Math.Sqrt
            (
                Math.Pow(XDbl - p.XDbl, 2) +
                Math.Pow(YDbl - p.YDbl, 2)
            );

        public static double GetDistance(Point2D<CoordType> p1, Point2D<CoordType> p2)
            => Math.Sqrt
            (
                Math.Pow(p1.XDbl - p2.XDbl, 2) +
                Math.Pow(p1.YDbl - p2.YDbl, 2)
            );

        public override string ToString() => $"{X};{Y}";

        public bool Equals(Point2D<CoordType>? other)
            => other is not null && XDbl.Equals(other.XDbl) && YDbl.Equals(other.YDbl);

        public override bool Equals(object? obj) => Equals(obj as Point2D<CoordType>);

        public override int GetHashCode() => (int)Math.Pow(XDbl, YDbl);
    }
}
