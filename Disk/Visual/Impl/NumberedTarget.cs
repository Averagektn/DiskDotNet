using Disk.Data.Impl;
using System.Windows;

namespace Disk.Visual.Impl
{
    internal class NumberedTarget : Target
    {
        public NumberedTarget(Point2D<int> center, int radius, Size iniSize) : base(center, radius, iniSize)
        {
        }
    }
}
