namespace Disk.Data.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="CoordType">
    /// 
    /// </typeparam>
    internal class PolarPoint<CoordType> : Point2D<CoordType> where CoordType : IConvertible, new()
    {
        /// <summary>
        /// 
        /// </summary>
        public double Radius
        {
            get => Math.Sqrt(Math.Pow(X.ToDouble(FormatProvider), 2) + Math.Pow(Y.ToDouble(FormatProvider), 2));
        }

        /// <summary>
        /// 
        /// </summary>
        public double Angle
        {
            get
            {
                var angleRad = Math.Atan2(YDbl, XDbl);

                if (angleRad < 0)
                {
                    angleRad += 2 * Math.PI;
                }

                return (angleRad * (180.0 / Math.PI) + 360.0) % 360.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius">
        /// 
        /// </param>
        /// <param name="angleRad">
        /// 
        /// </param>
        public PolarPoint(double radius, double angleRad)
        {
            X = (CoordType)Convert.ChangeType(radius * Math.Cos(angleRad), typeof(CoordType));
            Y = (CoordType)Convert.ChangeType(radius * Math.Sin(angleRad), typeof(CoordType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">
        /// 
        /// </param>
        public PolarPoint(Point2D<CoordType> p) : base(p.X, p.Y) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">
        /// 
        /// </param>
        /// <param name="y">
        /// 
        /// </param>
        /// <param name="formatProvider">
        /// 
        /// </param>
        public PolarPoint(CoordType x, CoordType y, IFormatProvider? formatProvider = null) : base(x, y, formatProvider) { }

        /// <summary>
        /// 
        /// </summary>
        public PolarPoint() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1">
        /// 
        /// </param>
        /// <param name="p2">
        /// 
        /// </param>
        /// <param name="formatProvider">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static double GetDistance(PolarPoint<CoordType> p1, PolarPoint<CoordType> p2,
            IFormatProvider? formatProvider = null)
            => Math.Sqrt
            (
                Math.Pow(p1.X.ToDouble(formatProvider) - p2.X.ToDouble(formatProvider), 2) +
                Math.Pow(p2.X.ToDouble(formatProvider) - p2.Y.ToDouble(formatProvider), 2)
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public double GetDistance(PolarPoint<CoordType> p) => GetDistance(new Point2D<CoordType>(p.X, p.Y));

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public override string ToString() => $"{Radius};{Angle}";

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<CoordType> To2D() => new(X, Y);
    }
}
