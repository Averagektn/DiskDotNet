using Disk.Data.Interface;

namespace Disk.Data.Impl
{
    internal class Point2DF : IPoint2D<float>
    {
        public Point2DF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point2DF() 
        {
            X = 0; 
            Y = 0;
        }

        public float X { get; set; }
        public float Y { get; set; }

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
