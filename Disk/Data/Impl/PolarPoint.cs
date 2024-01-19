namespace Disk.Data.Impl
{
    internal class PolarPoint<CoordType> : Point2D<CoordType> where CoordType : IConvertible, new()
    {
        public double Radius
        {
            get => Math.Sqrt(Math.Pow(X.ToDouble(FormatProvider), 2) + Math.Pow(Y.ToDouble(FormatProvider), 2));
        }

        public double Angle
        {
            get
            {
                var angleRad = Math.Atan2(YDbl, XDbl);

                if (angleRad < 0)
                {
                    angleRad += 2 * Math.PI;
                }

                return angleRad;
            }
        }

        public PolarPoint(double radius, double angleRad)
        {
            X = (CoordType)Convert.ChangeType(radius * Math.Cos(angleRad), typeof(CoordType));
            Y = (CoordType)Convert.ChangeType(radius * Math.Sin(angleRad), typeof(CoordType));
        }

        public PolarPoint(Point2D<CoordType> p) : base(p.X, p.Y) { }

        public PolarPoint(CoordType x, CoordType y, IFormatProvider? formatProvider = null) : base(x, y, formatProvider) { }

        public PolarPoint() : base() { }

        public static double GetDistance(PolarPoint<CoordType> p1, PolarPoint<CoordType> p2,
            IFormatProvider? formatProvider = null)
            => Math.Sqrt
            (
                Math.Pow(p1.X.ToDouble(formatProvider) - p2.X.ToDouble(formatProvider), 2) +
                Math.Pow(p2.X.ToDouble(formatProvider) - p2.Y.ToDouble(formatProvider), 2)
            );

        public double GetDistance(PolarPoint<CoordType> p) => GetDistance(new Point2D<CoordType>(p.X, p.Y));

        public override string ToString() => $"{Radius};{Angle}";

        public Point2D<CoordType> To2D() => new(X, Y);
    }
}
