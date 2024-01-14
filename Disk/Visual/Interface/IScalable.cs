using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk.Visual
{
    interface IScalable
    {
        Point CurrSize { get; }
        void Scale(Point newSize);
    }
}
