using Disk.Data.Impl;
using System.Windows;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    class Enemy : User
    {
        public Enemy(Point2D<int> center, int radius, int speed, Brush color) : base(center, radius, speed, color) { }

        public Enemy(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) :
            base(center, radius, speed, color, iniSize)
        { }

        public void Follow() { }
    }
}
