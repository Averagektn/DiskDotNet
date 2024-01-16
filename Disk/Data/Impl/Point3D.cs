using Disk.Data.Interface;

namespace Disk.Data.Impl
{
    internal class Point3D : Point2D, IPoint3D<int>
    {
        public Point3D() : base() 
        {
            Z = 0;
        }
        public Point3D(int x, int y, int z) : base(x, y)
        {
            Z = z;
        }
        public int Z { get; set; };

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
