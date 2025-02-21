using Disk.Data.Impl;

namespace Disk.Visual.Interface;

public interface ITarget
{
    int ReceiveShot(Point2D<int> shot);
}
