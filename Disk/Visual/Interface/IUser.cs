using Disk.Data.Impl;

namespace Disk.Visual.Interface
{
    internal interface IUser
    {
        event Action<Point2D<int>>? OnShot;

        void ClearOnShot();

        Point2D<int> Shot();
    }
}
