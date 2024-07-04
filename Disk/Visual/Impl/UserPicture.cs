using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Disk.Visual.Impl
{
    internal class UserPicture : User
    {
        public new int Right => (int)(Center.X + (_image.Width / 2));
        public new int Top => (int)(Center.Y - (_image.Height / 2));
        public new int Bottom => (int)(Center.Y + (_image.Height / 2));
        public new int Left => (int)(Center.X - (_image.Width / 2));

        protected Size CurrSize { get; private set; }
        protected const double DIAGONAL_CORRECTION = 1.41;

        private readonly Image _image;
        private readonly Size IniImageSize;
        protected readonly Size IniSize;

        public UserPicture(string filePath, Point2D<int> center, int speed, Size imageSize, Size iniSize)
            : base(center, 0, speed, new SolidColorBrush(Colors.Transparent), iniSize)
        {
            IniSize = iniSize;
            CurrSize = iniSize;
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

        public override void Draw(IAddChild addChild)
        {
            addChild.AddChild(_image);
        }

        public override void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
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

        public override void Move(Point2D<int> center)
        {
            if (center.X <= CurrSize.Width && center.Y <= CurrSize.Height && center.X > 0 && center.Y > 0)
            {
                Center = center;

                _image.Margin = new(Left, Top, 0, 0);
            }
        }

        public override void Remove(UIElementCollection collection)
        {
            collection.Remove(_image);
        }

        public override void Scale(Size newSize)
        {
            double coeffX = (double)newSize.Width / IniSize.Width;
            double coeffY = (double)newSize.Height / IniSize.Height;

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
