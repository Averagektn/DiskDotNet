using Disk.Data.Impl;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    class Target : User
    {
        private readonly List<Circle> Circles;

        public Target(Point2D<int> center, int radius) : base(center, radius, 0, Brushes.White)
        {
            Circles =
            [
                new(center, radius * 5, 0, Brushes.Red),
                new(center, radius * 4, 0, Brushes.White),
                new(center, radius * 3, 0, Brushes.Red),
                new(center, radius * 2, 0, Brushes.White),
                new(center, radius * 1, 0, Brushes.Red)
            ];
        }

        public Target(Point2D<int> center, int radius, Size iniSize) :
            base(center, radius, 0, Brushes.White, iniSize)
        {
            Circles =
            [
                new(center, radius * 5, 0, Brushes.Red, iniSize),
                new(center, radius * 4, 0, Brushes.White, iniSize),
                new(center, radius * 3, 0, Brushes.Red, iniSize),
                new(center, radius * 2, 0, Brushes.White, iniSize),
                new(center, radius * 1, 0, Brushes.Red, iniSize)
            ];
        }

        public override void Draw(IAddChild addChild)
        {
            foreach (var circle in Circles)
            {
                circle.Draw(addChild);
            }
        }

        public override void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            foreach (var circle in Circles)
            {
                circle.Move(moveTop, moveRight, moveBottom, moveLeft);
            }
        }

        public override void Scale(Size newSize)
        {
            foreach (var circle in Circles)
            {
                circle.Scale(newSize);
            }
        }

        public override void Move(Point2D<int> center)
        {
            foreach (var circle in Circles)
            {
                circle.Move(center);
            }
        }

        public int ReceiveShot(Point2D<int> shot)
        {
            int res = 0;

            foreach (var circle in Circles)
            {
                res += circle.Contains(shot) ? 5 : 0;
            }

            return res;
        }
    }
}
