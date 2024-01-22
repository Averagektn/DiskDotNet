using System.Drawing;
using Point = System.Windows.Point;

namespace Disk.Data.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="CoordType">
    /// 
    /// </typeparam>
    class Point2D<CoordType> : IEquatable<Point2D<CoordType>> where CoordType : IConvertible, new()
    {
        /// <summary>
        /// 
        /// </summary>
        public CoordType X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CoordType Y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double XDbl
        {
            get => X.ToDouble(FormatProvider);
        }

        /// <summary>
        /// 
        /// </summary>
        public double YDbl
        {
            get => Y.ToDouble(FormatProvider);
        }

        /// <summary>
        /// 
        /// </summary>
        protected readonly IFormatProvider? FormatProvider;

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
        public Point2D(CoordType x, CoordType y, IFormatProvider? formatProvider = null)
        {
            X = x;
            Y = y;
            FormatProvider = formatProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        public Point2D()
        {
            X = new();
            Y = new();
            FormatProvider = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public double GetDistance(Point2D<CoordType> p)
            => Math.Sqrt
            (
                Math.Pow(XDbl - p.XDbl, 2) +
                Math.Pow(YDbl - p.YDbl, 2)
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1">
        /// 
        /// </param>
        /// <param name="p2">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static double GetDistance(Point2D<CoordType> p1, Point2D<CoordType> p2)
            => Math.Sqrt
            (
                Math.Pow(p1.XDbl - p2.XDbl, 2) +
                Math.Pow(p1.YDbl - p2.YDbl, 2)
            );

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public override string ToString() => $"{X};{Y}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public bool Equals(Point2D<CoordType>? other)
            => other is not null && XDbl.Equals(other.XDbl) && YDbl.Equals(other.YDbl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public override bool Equals(object? obj) => Equals(obj as Point2D<CoordType>);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public override int GetHashCode() => (int)Math.Pow(XDbl, YDbl);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point ToPoint() => new(XDbl, YDbl);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public PointF ToPointF() => new((float)XDbl, (float)YDbl);
    }
}
