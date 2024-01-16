using Disk.Data.Interface;

namespace Disk.Data.Impl
{
    internal class Point3DF : Point2DF, IPoint3D<float>
    {
        public float Z { get; set; }

        public Point3DF() : base()
        {
            Z = 0;
        }

        public Point3DF(float x, float y, float z) : base(x, y)
        {
            Z = z;
        }

        public static double GetDistance(Point3DF p)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return $"{X};{Y};{Z}";
        }
    }
}
