using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace Disk.Visual.Impl
{
    internal class UserPicture : IFigure
    {
        public int Right => (int)(Center.X + _image.Width / 2);

        public int Top => (int)(Center.Y - _image.Height / 2);

        public int Bottom => (int)(Center.Y + _image.Height / 2);

        public int Left => (int)(Center.X - _image.Width / 2);
        protected const double DIAGONAL_CORRECTION = 1.41;

        public Point2D<int> Center { get; private set; }

        private readonly Image _image;
        private int Speed;
        private readonly int IniSpeed;
        private readonly Size IniImageSize;
        protected readonly Size IniSize;
        protected Size CurrSize { get; private set; }

        public event Action<Point2D<int>>? OnShot;

        public void ClearOnShot()
        {
            OnShot = null;
        }

        public Point2D<int> Shot()
        {
            OnShot?.Invoke(Center);

            return Center;
        }

        public UserPicture(string filePath, Point2D<int> center, int speed, Size imageSize, Size iniSize)
        {
            Center = center;
            Speed = speed;
            IniSize = iniSize;
            CurrSize = iniSize;
            IniSpeed = speed;
            IniImageSize = imageSize;
            _image = new()
            {
                Source = new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute)),
                Width = imageSize.Width,
                Height = imageSize.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
        }

        public void Draw(IAddChild addChild)
        {
            addChild.AddChild(_image);
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

            _image.Margin = new(Left, Top, 0, 0);
        }

        public void Move(Point2D<int> center)
        {
            if (center.X <= CurrSize.Width && center.Y <= CurrSize.Height && center.X > 0 && center.Y > 0)
            {
                Center = center;

                _image.Margin = new(Left, Top, 0, 0);
            }
        }

        public void Remove(UIElementCollection collection)
        {
            collection.Remove(_image);
        }

        public void Scale(Size newSize)
        {
            double coeffX = (double)newSize.Width / IniSize.Width;
            double coeffY = (double)newSize.Height / IniSize.Height;

            Speed = (int)Math.Round(IniSpeed * (coeffX + coeffY) / 2);

            Center = new(
                    (int)Math.Round(Center.X * (newSize.Width / CurrSize.Width)),
                    (int)Math.Round(Center.Y * (newSize.Height / CurrSize.Height))
                );

            _image.Width = IniImageSize.Width * coeffX;
            _image.Height = IniImageSize.Height * coeffY;

            CurrSize = newSize;

            _image.Margin = new(Left, Top, 0, 0);
        }
    }
}
