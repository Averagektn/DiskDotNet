namespace Disk.Data.Interface
{
    internal interface IPoint2D<CoordType> where CoordType :
        IComparable, 
        IFormattable, 
        IConvertible, 
        IComparable<CoordType>, 
        IEquatable<CoordType>
    {
        public CoordType X { get; set; }
        public CoordType Y { get; set; }
    }
}
