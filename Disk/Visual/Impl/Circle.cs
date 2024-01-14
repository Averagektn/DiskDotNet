using System.Drawing;

namespace Disk.Visual
{
    class Circle : IFigure
    {
        private Point Center;
        private int Radius;
        private const double DIAGONAL_CORRECTION = 1.41;
        protected int Speed;
        public int X => Center.X;

        public int Y => Center.Y;

        public int Right => Center.X + Radius;

        public int Top => Center.Y - Radius;

        public int Bottom => Center.Y + Radius;

        public int Left => Center.X - Radius;

        public Point CurrSize => throw new NotImplementedException();

        public Circle(Point center, int radius, int speed)
        {
            Center = center;
            Radius = radius;
            Speed = speed;
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            throw new NotImplementedException();
        }

        public void Scale(Point newSize)
        {
            throw new NotImplementedException();
        }
    }
}
