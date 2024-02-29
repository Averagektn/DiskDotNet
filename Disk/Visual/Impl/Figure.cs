using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Disk.Visual.Impl
{
    internal abstract class Figure<TFigure> : IFigure where TFigure : FrameworkElement, new()
    {
        private readonly double DIAGONAL_CORRECTION = Math.Sqrt(2);
        public Point2D<int> Center { get; protected set; }

        public int Right => throw new NotImplementedException();

        public int Top => throw new NotImplementedException();

        public int Bottom => throw new NotImplementedException();

        public int Left => throw new NotImplementedException();
        protected int Speed;
        private int _iniSpeed;
        protected Size CurrSize;
        private Size IniSize;
        private int IniRadius;
        public int Radius { get; protected set; }

        protected readonly TFigure _figure;

        public Figure(Point2D<int> center)
        {
            _figure = new();
            Center = center;
        }

        public void Draw(IAddChild addChild)
        {
            addChild.AddChild(_figure);
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

            _figure.Margin = new(Left, Top, 0, 0);
        }

        public void Move(Point2D<int> center)
        {
            if (center.X <= CurrSize.Width && center.Y <= CurrSize.Height && center.X > 0 && center.Y > 0)
            {
                Center = center;

                _figure.Margin = new(Left, Top, 0, 0);
            }
        }

        public void Remove(UIElementCollection collection)
        {
            collection.Remove(_figure);
        }

        public void Scale(Size newSize)
        {
            double coeffX = (double)newSize.Width / IniSize.Width;
            double coeffY = (double)newSize.Height / IniSize.Height;

            Speed = (int)Math.Round(_iniSpeed * (coeffX + coeffY) / 2);
            Radius = (int)Math.Round(IniRadius * (coeffX + coeffY) / 2);

            Center = new(
                    (int)Math.Round(Center.X * (newSize.Width / CurrSize.Width)),
                    (int)Math.Round(Center.Y * (newSize.Height / CurrSize.Height))
                );

            _figure.Width = Radius * 2;
            _figure.Height = Radius * 2;

            CurrSize = newSize;

            _figure.Margin = new(Left, Top, 0, 0);
        }
    }
}
