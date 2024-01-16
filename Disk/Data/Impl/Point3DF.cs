namespace Disk.Data.Impl
{
    internal class Point3DF(float x, float y, float z) : Point2DF(x, y)
    {
        public float Z { get; } = z;

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
