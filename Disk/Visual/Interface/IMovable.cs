using Disk.Data.Impl;

namespace Disk.Visual.Interface
{
    internal interface IMovable
    {
        void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft);

        void Move(Point2D<int> center);
    }
}
