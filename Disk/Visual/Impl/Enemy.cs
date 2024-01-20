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

        public void Follow(Point2D<int> target)
        {
            var direction = GetAngleDirection(target);

            bool moveRight = IsBetween(direction, 315, 45);
            bool moveTop = IsBetween(direction, 45, 135);
            bool moveLeft = IsBetween(direction, 135, 225);
            bool moveBottom = IsBetween(direction, 225, 315);

            Move(moveTop, moveRight, moveBottom, moveLeft);
        }

        private float GetAngleDirection(Point2D<int> target)
        {
            var deltaX = target.X - Center.X;
            var deltaY = -target.Y + Center.Y;

            return (float)new PolarPoint<double>(new Point2D<double>(deltaX, deltaY)).Angle;

            /*            var angleRadians = Math.Atan2(deltaY, deltaX);

                        var angleDegrees = angleRadians * (180 / Math.PI);

                        if (angleDegrees < 0)
                        {
                            angleDegrees += 360;
                        }

                        return (float)angleDegrees;*/
        }

        private static bool IsBetween(float angle, float left, float right)
        {
            angle = (angle + 360.0f) % 360.0f;
            left = (left + 360.0f) % 360.0f;
            right = (right + 360.0f) % 360.0f;

            if (left <= right)
            {
                return angle >= left && angle <= right;
            }
            else
            {
                return angle >= left || angle <= right;
            }
        }
    }
}
