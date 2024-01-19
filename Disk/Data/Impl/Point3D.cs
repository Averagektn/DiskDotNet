namespace Disk.Data.Impl
{
    class Point3D<CoordType> : Point2D<CoordType> where CoordType : IConvertible, new()
    {
        public CoordType Z { get; set; }
        public double ZDbl
        {
            get => Z.ToDouble(FormatProvider);
        }

        public Point3D(CoordType x, CoordType y, CoordType z, IFormatProvider? formatProvider = null) : 
            base(x, y, formatProvider)
        {
            Z = z;
        }

        public Point3D()
        {
            Z = new();
        }

        public double GetDistance(Point3D<CoordType> p)
            => Math.Sqrt
            (
                Math.Pow(XDbl - p.XDbl, 2) +
                Math.Pow(YDbl - p.YDbl, 2) +
                Math.Pow(ZDbl - p.ZDbl, 2)
            );

        public static double GetDistance(Point3D<CoordType> p1, Point3D<CoordType> p2, IFormatProvider? formatProvider = null)
            => Math.Sqrt
            (
                Math.Pow(p1.XDbl - p2.XDbl, 2) + 
                Math.Pow(p1.YDbl - p2.YDbl, 2) + 
                Math.Pow(p1.ZDbl - p2.ZDbl, 2)
            );

        public override string ToString() => $"{X};{Y};{Z}";

        public Point2D<CoordType> To2D() => new(X, Y);
    }
}
