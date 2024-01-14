using Disk.Visual.Interface;

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
