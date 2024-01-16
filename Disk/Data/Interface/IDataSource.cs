namespace Disk.Data.Interface
{
    internal interface IDataSource<TriplePoint, DoublePoint, CoordType>
        where TriplePoint :
            IPoint<CoordType>
        where CoordType :
            IComparable,
            IFormattable,
            IConvertible,
            IComparable<CoordType>,
            IEquatable<CoordType>
    {
        TriplePoint GetXYZ();
        DoublePoint GetXY();
        DoublePoint GetYZ();
        DoublePoint GetXZ();
        DoublePoint GetYX();
        DoublePoint GetZY();
        DoublePoint GetZX();
    }
}
