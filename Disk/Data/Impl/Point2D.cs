namespace Disk.Data.Impl
{
    internal class Point2D<CoordType> where CoordType : new()
    {
        public CoordType X { get; set; }

        public CoordType Y { get; set; }

        public Point2D(CoordType x, CoordType y)
        {
            X = x;
            Y = y;
        }

        public Point2D()
        {
            X = new();
            Y = new();
        }

        public static double GetDistance(Point2D<CoordType> p)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => $"{X};{Y}";
    }
}
