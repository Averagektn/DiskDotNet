using System.Drawing;

namespace Disk.Visual
{
    interface IScalable
    {
        Point CurrSize { get; }
        void Scale(Point newSize);
    }
}
