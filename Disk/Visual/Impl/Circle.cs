using System.Drawing;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Disk.Visual
{
    class Circle : IFigure
    {
        private readonly Ellipse Figure;
        public Point Center { get; protected set; }
        protected int Radius;
        private const float DIAGONAL_CORRECTION = 1.41f;
        protected int Speed;
        public int X => Center.X;

        public int Y => Center.Y;

        public int Right => Center.X + Radius;

        public int Top => Center.Y - Radius;

        public int Bottom => Center.Y + Radius;

        public int Left => Center.X - Radius;
        private Point IniSize { get; set; }

        private Point CurrSize { get; set; }

        private bool isDrawn = false;

        public Circle(Point center, int radius, int speed, Brush color)
        {
            Center = center;
            Radius = radius;
            Speed = speed;

            Figure = new()
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = color  
            };
        }

        public Circle(Point center, int radius, int speed, Brush color, Point iniSize) : this(center, radius, speed, color)
        {
            CurrSize = iniSize;
            IniSize = iniSize;
        }

        public void Draw(IAddChild addChild)
        {
            if (!isDrawn)
            {
                addChild.AddChild(Figure);
                isDrawn = true;
            }
        }

        public void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            int xSpeed = 0;
            int ySpeed = 0;
            int speed = Speed;

            if ((moveTop || moveBottom) && (moveRight || moveLeft))
            {
                speed = (int)Math.Round((float)speed / DIAGONAL_CORRECTION);
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
            if (Right >= CurrSize.X && xSpeed > 0)
            {
                xSpeed = 0;
            }
            if (Top <= 0 && ySpeed < 0)
            {
                ySpeed = 0;
            }
            if (Bottom >= CurrSize.Y && ySpeed > 0)
            {
                ySpeed = 0;
            }

            Center = new(Center.X + xSpeed, Center.Y + ySpeed);

            Canvas.SetLeft(Figure, Left);
            Canvas.SetTop(Figure, Top);
        }

        public void Scale(Point newSize)
        {
            float coeffX = (float)newSize.X / IniSize.X;
            float coeffY = (float)newSize.Y / IniSize.Y;

            Speed = (int)Math.Round(Speed * (coeffX + coeffY) / 2);
            Radius = (int)Math.Round(Radius * (coeffX + coeffY) / 2);
            Center = new((int)Math.Round(Center.X * coeffX), (int)Math.Round(Center.Y * coeffY));

            Canvas.SetLeft(Figure, Left);
            Canvas.SetTop(Figure, Top);
        }
    }
}
