namespace Disk.Data.Impl
{
    /// <summary>
    ///     Represents a point in polar coordinates with coordinates of type <typeparamref name="CoordType"/>
    /// </summary>
    /// <typeparam name="CoordType">
    ///     The type of the coordinates
    /// </typeparam>
    internal class PolarPoint<CoordType> : Point2D<CoordType> where CoordType : IConvertible, new()
    {
        /// <summary>
        ///     Gets the radius of the point in polar coordinates
        /// </summary>
        public double Radius => Math.Sqrt(Math.Pow(X.ToDouble(FormatProvider), 2) + Math.Pow(Y.ToDouble(FormatProvider), 2));

        /// <summary>
        ///     Gets the angle of the point in polar coordinates
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
        ///     Initializes a new instance of the <see cref="PolarPoint{CoordType}"/> class with the specified radius and 
        ///     angle
        /// </summary>
        /// <param name="radius">
        ///     The radius coordinate of the point in polar coordinates
        /// </param>
        /// <param name="angleRad">
        ///     The angle coordinate of the point in polar coordinates (in radians)
        /// </param>
        public PolarPoint(double radius, double angleRad)
        {
            X = (CoordType)Convert.ChangeType(radius * Math.Cos(angleRad), typeof(CoordType));
            Y = (CoordType)Convert.ChangeType(radius * Math.Sin(angleRad), typeof(CoordType));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PolarPoint{CoordType}"/> class with the coordinates of the 
        ///     specified two-dimensional point
        /// </summary>
        /// <param name="p">
        ///     The two-dimensional point to initialize the polar point from
        /// </param>
        public PolarPoint(Point2D<CoordType> p) : base(p.X, p.Y) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PolarPoint{CoordType}"/> class with the specified coordinates 
        ///     and format provider
        /// </summary>
        /// <param name="x">
        ///     The X coordinate of the point
        /// </param>
        /// <param name="y">
        ///     The Y coordinate of the point
        /// </param>
        /// <param name="formatProvider">
        ///     The format provider used for converting the coordinates to double values. (Optional)
        /// </param>
        public PolarPoint(CoordType x, CoordType y, IFormatProvider? formatProvider = null) : base(x, y, formatProvider) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PolarPoint{CoordType}"/> class with default coordinates and no 
        ///     format provider
        /// </summary>
        public PolarPoint() : base() { }

        /// <summary>
        ///     Calculates the Euclidean distance between two polar points
        /// </summary>
        /// <param name="p1">
        ///     The first polar point
        /// </param>
        /// <param name="p2">
        ///     The second polar point
        /// </param>
        /// <param name="formatProvider">
        ///     The format provider used for converting the coordinates to double values. (Optional)
        /// </param>
        /// <returns>
        ///     The Euclidean distance between the two polar points
        /// </returns>
        public static double GetDistance(PolarPoint<CoordType> p1, PolarPoint<CoordType> p2,
            IFormatProvider? formatProvider = null)
            => Math.Sqrt
            (
                Math.Pow(p1.X.ToDouble(formatProvider) - p2.X.ToDouble(formatProvider), 2) +
                Math.Pow(p1.X.ToDouble(formatProvider) - p2.Y.ToDouble(formatProvider), 2)
            );

        /// <summary>
        ///     Calculates the Euclidean distance between this polar point and the specified polar point
        /// </summary>
        /// <param name="p">
        ///     The polar point to calculate the distance to
        /// </param>
        /// <returns>
        ///     The Euclidean distance between this polar point and the specified polar point
        /// </returns>
        public double GetDistance(PolarPoint<CoordType> p) => GetDistance(new Point2D<CoordType>(p.X, p.Y));

        /// <summary>
        ///     Returns a string representation of the polar point in the format "Radius;Angle"
        /// </summary>
        /// <returns>
        ///     A string representation of the polar point
        /// </returns>
        public override string ToString() => $"{Radius};{Angle}";

        /// <summary>
        ///     Converts the polar point to a two-dimensional point (<see cref="Point2D{CoordType}"/>)
        /// </summary>
        /// <returns>
        ///     A two-dimensional point representing the same coordinates as the polar point
        /// </returns>
        public Point2D<CoordType> To2D() => new(X, Y);
    }
}
