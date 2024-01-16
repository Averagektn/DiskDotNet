using Disk.Data.Interface;

namespace Disk.Data.Impl
{
    internal class Point2DF(float x, float y) : IPoint<float>
    {
        public float X { get; } = x;
        public float Y { get; } = y;

        public static double GetDistance(Point2DF p)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return $"{X};{Y}";
        }
    }
}
