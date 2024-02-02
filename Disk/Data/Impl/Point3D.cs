using System.Windows.Media.Media3D;

namespace Disk.Data.Impl
{
    /// <summary>
    ///     Represents a three-dimensional point with coordinates of type <typeparamref name="CoordType"/>
    /// </summary>
    /// <typeparam name="CoordType">
    ///     The type of the coordinates
    /// </typeparam>
    class Point3D<CoordType> : Point2D<CoordType>, IEquatable<Point3D<CoordType>> where CoordType : IConvertible, new()
    {
        /// <summary>
        ///     Gets or sets the Z coordinate of the point
        /// </summary>
        public CoordType Z { get; set; }

        /// <summary>
        ///     Gets the Z coordinate of the point as a double value
        /// </summary>
        public double ZDbl => Z.ToDouble(FormatProvider);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D{CoordType}"/> class with the specified coordinates 
        ///     and format provider
        /// </summary>
        /// <param name="x">
        ///     The X coordinate of the point
        /// </param>
        /// <param name="y">
        ///     The Y coordinate of the point
        /// </param>
        /// <param name="z">
        ///     The Z coordinate of the point
        /// </param>
        /// <param name="formatProvider">
        ///     The format provider used for converting the coordinates to double values. (Optional)
        /// </param>
        public Point3D(CoordType x, CoordType y, CoordType z, IFormatProvider? formatProvider = null) :
            base(x, y, formatProvider)
        {
            Z = z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D{CoordType}"/> class with default coordinates and no 
        ///     format provider
        /// </summary>
        public Point3D() : base()
        {
            Z = new();
        }

        /// <summary>
        ///     Calculates the Euclidean distance between this point and the specified point
        /// </summary>
        /// <param name="p">
        ///     The point to calculate the distance to
        /// </param>
        /// <returns>
        ///     The Euclidean distance between this point and the specified point
        /// </returns>
        public double GetDistance(Point3D<CoordType> p)
            => Math.Sqrt
            (
                Math.Pow(XDbl - p.XDbl, 2) +
                Math.Pow(YDbl - p.YDbl, 2) +
                Math.Pow(ZDbl - p.ZDbl, 2)
            );

        /// <summary>
        ///     Calculates the Euclidean distance between two points
        /// </summary>
        /// <param name="p1">
        ///     The first point
        /// </param>
        /// <param name="p2">
        ///     The second point
        /// </param>
        /// <returns>
        ///     The Euclidean distance between the two points
        /// </returns>
        public static double GetDistance(Point3D<CoordType> p1, Point3D<CoordType> p2)
            => Math.Sqrt
            (
                Math.Pow(p1.XDbl - p2.XDbl, 2) +
                Math.Pow(p1.YDbl - p2.YDbl, 2) +
                Math.Pow(p1.ZDbl - p2.ZDbl, 2)
            );

        /// <summary>
        ///     Returns a string representation of the point in the format "X;Y;Z"
        /// </summary>
        /// <returns>
        ///     A string representation of the point
        /// </returns>
        public override string ToString() => $"{X};{Y};{Z}";

        /// <summary>
        ///     Converts the point to a two-dimensional point (<see cref="Point2D{CoordType}"/>) by discarding the Z 
        ///     coordinate
        /// </summary>
        /// <returns>
        ///     A <see cref="Point2D{CoordType}"/> object representing the point without the Z coordinate
        /// </returns>
        public Point2D<CoordType> To2D() => new(X, Y);

        /// <summary>
        ///     Determines whether this point is equal to the specified point
        /// </summary>
        /// <param name="other">
        ///     The point to compare with this point
        /// </param>
        /// <returns>
        ///     <c>true</c> if this point is equal to the specified point; otherwise, <c>false</c>
        /// </returns>
        public bool Equals(Point3D<CoordType>? other)
            => other is not null && XDbl.Equals(other.XDbl) && YDbl.Equals(other.YDbl) && ZDbl.Equals(other.ZDbl);

        /// <summary>
        ///     Determines whether this point is equal to the specified object
        /// </summary>
        /// <param name="obj">
        ///     The object to compare with this point
        /// </param>
        /// <returns>
        ///     <c>true</c> if this point is equal to the specified object; otherwise, <c>false</c>
        /// </returns>
        public override bool Equals(object? obj) => Equals(obj as Point3D<CoordType>);

        /// <summary>
        ///     Returns the hash code for this point
        /// </summary>
        /// <returns>
        ///     The hash code for this point
        /// </returns>
        public override int GetHashCode() => (int)(Math.Pow(XDbl, YDbl) * ZDbl);

        /// <summary>
        ///     Converts the point to a three-dimensional point (<see cref="Point3D"/>) by converting the coordinates to 
        ///     double values
        /// </summary>
        /// <returns>
        ///     A <see cref="Point3D"/> object representing the point with coordinates as double values
        /// </returns>
        public Point3D ToPoint3D() => new(XDbl, YDbl, ZDbl);
    }
}
