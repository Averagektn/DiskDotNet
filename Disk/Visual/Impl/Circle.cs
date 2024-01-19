using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Drawing.Size;

namespace Disk.Visual.Impl
{
    class Circle : IFigure
    {
        public Point2D<int> Center { get; protected set; }

        public int Right => Center.X + Radius;

        public int Top => Center.Y - Radius;

        public int Bottom => Center.Y + Radius;

        public int Left => Center.X - Radius;

        protected int Radius;

        protected int Speed;

        private readonly Ellipse Figure;

        private const float DIAGONAL_CORRECTION = 1.41f;

        private Size IniSize { get; set; }

        private Size CurrSize { get; set; }

        private bool isDrawn = false;

        public Circle(Point2D<int> center, int radius, int speed, Brush color)
        {
            Center = center;
            Radius = radius;
            Speed = speed;

            Figure = new()
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = color,
                Margin = new Thickness(Left, Top, 0, 0)
            };
        }

        public Circle(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) : this(center, radius, speed, color)
        {
            CurrSize = iniSize;
            IniSize = iniSize;
        }

        public virtual void Draw(IAddChild addChild)
        {
            if (!isDrawn)
            {
                addChild.AddChild(Figure);
                isDrawn = true;
            }

            Figure.Margin = new Thickness(Left, Top, 0, 0);
        }

        public void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            int xSpeed = 0;
            int ySpeed = 0;
            int speed = Speed;

            if ((moveTop || moveBottom) && (moveRight || moveLeft))
            {
                speed = (int)Math.Round(speed / DIAGONAL_CORRECTION);
            }

            if (moveTop)
            {
                ySpeed -= speed;
            }
            if (moveBottom)
            {
                ySpeed += speed;
            }
            if (moveLeft)
            {
                xSpeed -= speed;
            }
            if (moveRight)
            {
                xSpeed += speed;
            }

            if (Left <= 0 && xSpeed < 0)
            {
                xSpeed = 0;
            }
            if (Right >= CurrSize.Width && xSpeed > 0)
            {
                xSpeed = 0;
            }
            if (Top <= 0 && ySpeed < 0)
            {
                ySpeed = 0;
            }
            if (Bottom >= CurrSize.Height && ySpeed > 0)
            {
                ySpeed = 0;
            }

            Center = new(Center.X + xSpeed, Center.Y + ySpeed);

            Figure.Margin = new Thickness(Left, Top, 0, 0);
        }

        public void Scale(Size newSize)
        {
            float coeffX = (float)newSize.Width / IniSize.Width;
            float coeffY = (float)newSize.Height / IniSize.Height;

            Speed = (int)Math.Round(Speed * (coeffX + coeffY) / 2);
            Radius = (int)Math.Round(Radius * (coeffX + coeffY) / 2);
            Center = new((int)Math.Round(Center.X * coeffX), (int)Math.Round(Center.Y * coeffY));

            Figure.Margin = new Thickness(Left, Top, 0, 0);
        }
    }
}
