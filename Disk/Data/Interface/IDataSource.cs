using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
