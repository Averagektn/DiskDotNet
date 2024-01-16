namespace Disk.Data.Impl
{
    internal class Point3D(int x, int y, int z) : Point2D(x, y)
    {
        public int Z { get; } = z;

        public static double GetDistance(Point3D p)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return $"{X};{Y};{Z}";
        }
    }
}
