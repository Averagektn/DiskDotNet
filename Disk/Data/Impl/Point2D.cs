using System.Drawing;
using Point = System.Windows.Point;

namespace Disk.Data.Impl
{
    /// <summary>
    ///     Represents a two-dimensional point with coordinates of type <typeparamref name="CoordType"/>
    /// </summary>
    /// <typeparam name="CoordType">
    ///     The type of the coordinates
    /// </typeparam>
    public class Point2D<CoordType> : IEquatable<Point2D<CoordType>> where CoordType : IConvertible, new()
    {
        /// <summary>
        ///     Gets or sets the X coordinate of the point
        /// </summary>
        public CoordType X { get; set; }

        /// <summary>
        ///     Gets or sets the Y coordinate of the point
        /// </summary>
        public CoordType Y { get; set; }

        /// <summary>
        ///     Gets the X coordinate of the point as a double value
        /// </summary>
        public double XDbl => X.ToDouble(FormatProvider);

        /// <summary>
        ///     Gets the Y coordinate of the point as a double value
        /// </summary>
        public double YDbl => Y.ToDouble(FormatProvider);

        /// <summary>
        ///     The format provider used for converting the coordinates to double values
        /// </summary>
        protected readonly IFormatProvider? FormatProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point2D{CoordType}"/> 
        ///     class with the specified coordinates and format provider
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
        public Point2D(CoordType x, CoordType y, IFormatProvider? formatProvider = null)
        {
            X = x;
            Y = y;
            FormatProvider = formatProvider;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point2D{CoordType}"/> 
        ///     class with default coordinates and no format provider
        /// </summary>
        public Point2D()
        {
            X = new CoordType();
            Y = new CoordType();
            FormatProvider = null;
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
        public double GetDistance(Point2D<CoordType> p)
            => Math.Sqrt
            (
                Math.Pow(XDbl - p.XDbl, 2) +
                Math.Pow(YDbl - p.YDbl, 2)
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
        public static double GetDistance(Point2D<CoordType> p1, Point2D<CoordType> p2)
            => Math.Sqrt
            (
                Math.Pow(p1.XDbl - p2.XDbl, 2) +
                Math.Pow(p1.YDbl - p2.YDbl, 2)
            );

        /// <summary>
        ///     Returns a string representation of the point in the format "X;Y"
        /// </summary>
        /// <returns>
        ///     A string representation of the point
        /// </returns>
        public override string ToString() => $"{X};{Y}";

        /// <summary>
        ///     Determines whether this point is equal to the specified point
        /// </summary>
        /// <param name="other">
        ///     The point to compare with this point
        /// </param>
        /// <returns>
        ///     <c>true</c> if this point is equal to the specified point; otherwise, <c>false</c>
        /// </returns>
        public bool Equals(Point2D<CoordType>? other)
            => other is not null && XDbl.Equals(other.XDbl) && YDbl.Equals(other.YDbl);

        /// <summary>
        ///     Determines whether this point is equal to the specified object
        /// </summary>
        /// <param name="obj">
        ///     The object to compare with this point
        /// </param>
        /// <returns>
        ///     <c>true</c> if this point is equal to the specified object; otherwise, <c>false</c>
        /// </returns>
        public override bool Equals(object? obj) => Equals(obj as Point2D<CoordType>);

        /// <summary>
        ///     Returns the hash code for this point
        /// </summary>
        /// <returns>
        ///     The hash code for this point
        /// </returns>
        public override int GetHashCode() => (int)Math.Pow(XDbl, YDbl);

        /// <summary>
        ///     Converts the point to a <see cref="System.Drawing.Point"/> object
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Drawing.Point"/> object representing the point
        /// </returns>
        public Point ToPoint() => new((int)XDbl, (int)YDbl);

        /// <summary>
        ///     Converts the point to a <see cref="System.Drawing.PointF"/> object
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Drawing.PointF"/> object representing the point
        /// </returns>
        public PointF ToPointF() => new((float)XDbl, (float)YDbl);
    }
}
