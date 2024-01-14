using System.Drawing;

namespace Disk.Data
{
    interface IDataSource
    {
        Point GetXY();
        Point GetYZ();
        Point GetXZ();
        Point GetYX();
        Point GetZY();
        Point GetZX();
    }
}
