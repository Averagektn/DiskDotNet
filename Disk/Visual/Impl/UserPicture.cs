﻿using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Disk.Visual.Impl;

/// <summary>
///     User or cursor, containing image
/// </summary>
public class UserPicture : User
{
    /// <inheritdoc/>
    public override Point2D<int> Center
    {
        get => base.Center;
        protected set
        {
            base.Center = value;

            Canvas.SetLeft(Image, Left);
            Canvas.SetTop(Image, Top);
        }
    }

    /// <inheritdoc/>
    public override int Right => (int)(Center.X + (Image.Width / 2));

    /// <inheritdoc/>
    public override int Top => (int)(Center.Y - (Image.Height / 2));

    /// <inheritdoc/>
    public override int Bottom => (int)(Center.Y + (Image.Height / 2));

    /// <inheritdoc/>
    public override int Left => (int)(Center.X - (Image.Width / 2));

    /// <summary>
    ///     Image to be drawn
    /// </summary>
    protected readonly Image Image;

    /// <summary>
    ///     Initial size of the image
    /// </summary>
    protected readonly Size IniImageSize;

    /// <summary>
    ///     User or cursor, containing image
    /// </summary>
    /// <param name="filePath">
    ///     Path to image
    /// </param>
    /// <param name="center">
    ///     The center point of the target
    /// </param>
    /// <param name="speed">
    ///     The speed of the circle
    /// </param>
    /// <param name="imageSize">
    ///     Initial size of the image
    /// </param>
    /// <param name="parent">
    ///     Canvas, containing all figures
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the target
    /// </param>
    public UserPicture(string filePath, Point2D<int> center, int speed, Size imageSize, Canvas parent, Size iniSize)
        : base(center, radius: (int)Math.Min(imageSize.Width / 2, imageSize.Height / 2), speed, Brushes.Transparent,
            parent, iniSize)
    {
        IniImageSize = imageSize;
        Image = new()
        {
            Source = new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute)),
            Width = imageSize.Width,
            Height = imageSize.Height,
        };
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        base.Draw();

        _ = Parent.Children.Add(Image);
    }

    /// <inheritdoc/>
    public override void Remove()
    {
        base.Remove();

        Parent.Children.Remove(Image);
    }

    /// <inheritdoc/>
    public override void Scale()
    {
        double coeffX = (double)Parent.RenderSize.Width / IniSize.Width;
        double coeffY = (double)Parent.RenderSize.Height / IniSize.Height;

        Image.Width = (int)Math.Round(IniImageSize.Width * Math.Min(coeffX, coeffY));
        Image.Height = (int)Math.Round(IniImageSize.Height * Math.Min(coeffX, coeffY));

        base.Scale();
    }
}
