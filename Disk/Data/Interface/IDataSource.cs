namespace Disk.Data.Interface
{
    internal interface IDataSource<TriplePoint, DoublePoint>
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
