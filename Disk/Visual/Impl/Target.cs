using Disk.Data.Impl;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    class Target : User
    {
        public Target(Point2D<int> center, int radius, int speed, Brush color) : base(center, radius, speed, color) { }

        public Target(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) :
            base(center, radius, speed, color, iniSize)
        { }

        public override void Draw(IAddChild addChild)
        {

        }
    }
}
