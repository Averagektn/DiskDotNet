using Disk.Data.Impl;

namespace Disk.Data.Interface
{
    interface IDataSource<TriplePoint, DoublePoint, CoordType>
        where TriplePoint :
            Point3D<CoordType>
        where DoublePoint :
            Point2D<CoordType>
        where CoordType :
            IConvertible,
            new()
    {
        TriplePoint? GetXYZ();
        DoublePoint? GetXY();
        DoublePoint? GetYZ();
        DoublePoint? GetXZ();
        DoublePoint? GetYX();
        DoublePoint? GetZY();
        DoublePoint? GetZX();
    }
}
