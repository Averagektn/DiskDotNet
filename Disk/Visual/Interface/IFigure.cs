using Disk.Visual.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk.Visual
{
    interface IFigure : IDrawable, IScalable, IMovable
    {
        int X { get; }
        int Y { get; }
        int Right { get; }
        int Top { get; }
        int Bottom { get; }
        int Left { get; }
    }
}
