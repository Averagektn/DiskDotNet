using System.Windows.Media.Media3D;

namespace Disk.Data.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="CoordType">
    /// 
    /// </typeparam>
    class Point3D<CoordType> : Point2D<CoordType>, IEquatable<Point3D<CoordType>> where CoordType : IConvertible, new()
    {
        /// <summary>
        /// 
        /// </summary>
        public CoordType Z { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double ZDbl
        {
            get => Z.ToDouble(FormatProvider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">
        /// 
        /// </param>
        /// <param name="y">
        /// 
        /// </param>
        /// <param name="z">
        /// 
        /// </param>
        /// <param name="formatProvider">
        /// 
        /// </param>
        public Point3D(CoordType x, CoordType y, CoordType z, IFormatProvider? formatProvider = null) :
            base(x, y, formatProvider)
        {
            Z = z;
        }

        /// <summary>
        /// 
        /// </summary>
        public Point3D() : base()
        {
            Z = new();
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
        public double GetDistance(Point3D<CoordType> p)
            => Math.Sqrt
            (
                Math.Pow(XDbl - p.XDbl, 2) +
                Math.Pow(YDbl - p.YDbl, 2) +
                Math.Pow(ZDbl - p.ZDbl, 2)
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
        public static double GetDistance(Point3D<CoordType> p1, Point3D<CoordType> p2)
            => Math.Sqrt
            (
                Math.Pow(p1.XDbl - p2.XDbl, 2) +
                Math.Pow(p1.YDbl - p2.YDbl, 2) +
                Math.Pow(p1.ZDbl - p2.ZDbl, 2)
            );

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public override string ToString() => $"{X};{Y};{Z}";

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<CoordType> To2D() => new(X, Y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public bool Equals(Point3D<CoordType>? other)
            => other is not null && XDbl.Equals(other.XDbl) && YDbl.Equals(other.YDbl) && ZDbl.Equals(other.ZDbl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public override bool Equals(object? obj) => Equals(obj as Point3D<CoordType>);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public override int GetHashCode() => (int)(Math.Pow(XDbl, YDbl) * ZDbl);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point3D ToPoint3D() => new(XDbl, YDbl, ZDbl);
    }
}
