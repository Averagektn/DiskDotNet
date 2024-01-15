using System.Drawing;
using System.Windows.Media.Media3D;

namespace Disk.Data.Interface
{
    internal interface IDataSource
    {
        Point3D GetXYZ();
        Point GetXY();
        Point GetYZ();
        Point GetXZ();
        Point GetYX();
        Point GetZY();
        Point GetZX();
    }
}
