namespace Disk.Data.Interface
{
    internal interface IPoint3D<CoordType> : IPoint2D<CoordType>
        where CoordType :
            IComparable,
            IFormattable,
            IConvertible,
            IComparable<CoordType>,
            IEquatable<CoordType>
    {
        public CoordType Z { get; set; }
    }
}
