﻿using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Disk.Visual.Impl;

internal class UserPicture : User
{
    public override Point2D<int> Center
    {
        get => base.Center;
        protected set
        {
            base.Center = value;

            if (_image is not null)
            {
                Canvas.SetLeft(_image, Left);
                Canvas.SetTop(_image, Top);
            }
        }
    }

    public override int Right => (int)(Center.X + (_image?.Width ?? 0 / 2));
    public override int Top => (int)(Center.Y - (_image?.Height ?? 0 / 2));
    public override int Bottom => (int)(Center.Y + (_image?.Height ?? 0 / 2));
    public override int Left => (int)(Center.X - (_image?.Width ?? 0 / 2));

    private readonly Image _image;
    private readonly Size IniImageSize;

    public UserPicture(string filePath, Point2D<int> center, int speed, Size imageSize, Canvas parent, Size iniSize)
        : base(center, 0, speed, new SolidColorBrush(Colors.Transparent), parent, iniSize)
    {
        IniImageSize = imageSize;
        _image = new()
        {
            Source = new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute)),
            Width = imageSize.Width,
            Height = imageSize.Height,
            Stretch = Stretch.UniformToFill,
        };
    }

    public override void Draw()
    {
        _ = Parent.Children.Add(_image);
    }

    public override void Remove()
    {
        Parent.Children.Remove(_image);
    }

    public override void Scale()
    {
        double coeffX = (double)Parent.RenderSize.Width / IniSize.Width;
        double coeffY = (double)Parent.RenderSize.Height / IniSize.Height;

        Center = new(
                (int)Math.Round(Center.X * coeffX),
                (int)Math.Round(Center.Y * coeffY));

        _image.Width = IniImageSize.Width * (coeffX + coeffY) / 2;
        _image.Height = IniImageSize.Height * (coeffX + coeffY) / 2;
    }
}
