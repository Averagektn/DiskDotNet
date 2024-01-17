using Disk.Data.Impl;

namespace Disk.Data.Interface
{
    internal interface IDataSource<TriplePoint, DoublePoint, CoordType>
        where TriplePoint :
            Point3D<CoordType>
        where DoublePoint : 
            Point2D<CoordType>
        where CoordType:
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
