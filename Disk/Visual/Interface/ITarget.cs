using Disk.Data.Impl;

namespace Disk.Visual.Interface
{
    internal interface ITarget
    {
        int ReceiveShot(Point2D<int> shot);
    }
}
