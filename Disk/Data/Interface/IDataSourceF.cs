using System.Drawing;
using System.Windows.Media.Media3D;

namespace Disk.Data
{
    interface IDataSourceF
    {
        Point3D GetXYZ();
        PointF GetXY();
        PointF GetYZ();
        PointF GetXZ();
        PointF GetYX();
        PointF GetZY();
        PointF GetZX();
    }
}
