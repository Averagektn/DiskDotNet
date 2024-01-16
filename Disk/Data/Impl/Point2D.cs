using Disk.Data.Interface;

namespace Disk.Data.Impl
{
    internal class Point2D(int x, int y) : IPoint<int>
    {
        public int X { get; } = x;
        public int Y { get; } = y;

        public static double GetDistance(Point2D p)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return $"{X};{Y}";
        }
    }
}
