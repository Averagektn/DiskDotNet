using Disk.Data.Impl;

namespace Disk.Data.Interface
{
    interface IDataSource<CoordType>
        where CoordType :
            IConvertible,
            new()
    {
        Point3D<CoordType>? GetXYZ();

        Point2D<CoordType>? GetXY();

        Point2D<CoordType>? GetYZ();

        Point2D<CoordType>? GetXZ();

        Point2D<CoordType>? GetYX();

        Point2D<CoordType>? GetZY();

        Point2D<CoordType>? GetZX();
    }
}
