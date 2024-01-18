namespace Disk.Data.Impl
{
    internal class Point3D<CoordType> : Point2D<CoordType> where CoordType : new()
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

        public static double GetDistance(Point3D<CoordType> p)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => $"{X};{Y};{Z}";
    }
}
