using Disk.Data.Interface;

namespace Disk.Data.Impl
{
    internal class Point2D: IPoint2D<int>
    {
        public Point2D()
        {
            X = 0;
            Y = 0;
        }

        public Point2D(int x, int y)
        {
            X = x; 
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }

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
