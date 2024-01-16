namespace Disk.Data.Interface
{
    internal interface IPoint<CoordType> where CoordType :
        IComparable, 
        IFormattable, 
        IConvertible, 
        IComparable<CoordType>, 
        IEquatable<CoordType>
    {
        public CoordType X { get; }
        public CoordType Y { get; }
    }
}
