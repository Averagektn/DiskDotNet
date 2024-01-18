namespace Disk.Data.Impl
{
    internal class Point3D<CoordType> : Point2D<CoordType> where CoordType : IConvertible, new()
    {
        public CoordType Z { get; set; }

        public Point3D(CoordType x, CoordType y, CoordType z) : base(x, y)
        {
            Z = z;
        }

        public Point3D()
        {
            Z = new();
        }

        public static double GetDistance(Point3D<CoordType> p, IFormatProvider? formatProvider = null)
            => Math.Sqrt(
                Math.Pow(p.X.ToDouble(formatProvider), 2) + 
                Math.Pow(p.Y.ToDouble(formatProvider), 2) + 
                Math.Pow(p.Z.ToDouble(formatProvider), 2)
               );

        public override string ToString() => $"{X};{Y};{Z}";

        public Point2D<CoordType> To2D() => new(X, Y);
    }
}
