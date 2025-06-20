﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Disk.Data.Impl;
using Disk.Visual.Interfaces;

namespace Disk.Visual.Implementations;

/// <summary>
///     Represents a pointed path with a list of points, color, and initial size.
/// </summary>
public unsafe class PointedPath : IStaticFigure
{
    /// <summary>
    ///    Shows if the figure is drawn 
    ///    Protects from multiple <see cref="Draw"/> calls
    /// </summary>
    public bool IsDrawn { get; private set; } = false;

    /// <summary>
    ///    The color of the points
    /// </summary>
    public Color Color { get; private set; }

    /// <summary>
    ///   The radius of the points
    /// </summary>
    public int PointRadius { get; protected set; }

    /// <summary>
    ///     Initial radius of the points for scaling
    /// </summary>
    protected readonly int IniRadius;

    /// <summary>
    ///     Drawing area
    /// </summary>
    protected readonly Panel Parent;

    /// <summary>
    ///     The initial size of the drawing area for scaling
    /// </summary>
    protected readonly Size IniSize;

    private WriteableBitmap _bitmap;
    private IntPtr _backBuffer;
    private int _backBufferStride;

    private readonly List<Point2D<int>> _iniPoints;
    private readonly List<Point2D<int>> _points;
    private readonly Image _image;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PointedPath"/> class with the specified points, color, parent panel,
    /// </summary>
    /// <param name="points"></param>
    /// <param name="color"></param>
    /// <param name="parent"></param>
    /// <param name="iniSize"></param>
    /// <param name="pointRadius"></param>
    public PointedPath(IEnumerable<Point2D<int>> points, Color color, Panel parent, Size iniSize, int pointRadius = 2)
    {
        _iniPoints = [.. points];
        _points = [.. points];
        Color = color;
        Parent = parent;

        _bitmap = new((int)parent.ActualWidth, (int)parent.ActualHeight, dpiX: 96, dpiY: 96, PixelFormats.Pbgra32, null);
        _backBuffer = _bitmap.BackBuffer;
        _backBufferStride = _bitmap.BackBufferStride;
        IniSize = iniSize;

        _image = new Image()
        {
            Source = _bitmap,
            Width = _bitmap.Width,
            Height = _bitmap.Height,
        };

        PointRadius = pointRadius;
        IniRadius = pointRadius;
    }

    private void ModifyBitmap()
    {
        if (_points == null || _points.Count == 0)
        {
            return;
        }

        _bitmap.Lock();

        try
        {
            uint colorValue = (uint)((Color.A << 24) | (Color.R << 16) | (Color.G << 8) | Color.B);

            foreach (Point2D<int> center in _points)
            {
                int cx = center.X;
                int cy = center.Y;

                int x = 0;
                int y = PointRadius;
                int d = 3 - (2 * PointRadius);

                void DrawHorizontalLine(int x0, int x1, int y)
                {
                    if (x0 > x1)
                    {
                        (x0, x1) = (x1, x0);
                    }

                    for (int i = x0; i <= x1; i++)
                    {
                        if (i >= 0 && i < _bitmap.PixelWidth && y >= 0 && y < _bitmap.PixelHeight)
                        {
                            uint* pixel = (uint*)(_backBuffer + (y * _backBufferStride) + (i * 4));
                            *pixel = colorValue;
                        }
                    }
                }

                while (y >= x)
                {
                    DrawHorizontalLine(cx - x, cx + x, cy + y);
                    DrawHorizontalLine(cx - x, cx + x, cy - y);
                    DrawHorizontalLine(cx - y, cx + y, cy + x);
                    DrawHorizontalLine(cx - y, cx + y, cy - x);

                    x++;
                    if (d > 0)
                    {
                        y--;
                        d = d + (4 * (x - y)) + 10;
                    }
                    else
                    {
                        d = d + (4 * x) + 6;
                    }
                }
            }

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
        }
        finally
        {
            _bitmap.Unlock();
        }
    }

    private void Clear()
    {
        _bitmap.Lock();

        try
        {
            uint* pixels = (uint*)_backBuffer;
            const uint transparentColor = 0;

            for (int i = 0; i < _bitmap.PixelWidth * _bitmap.PixelHeight; i++)
            {
                pixels[i] = transparentColor;
            }

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
        }
        finally
        {
            _bitmap.Unlock();
        }
    }

    /// <inheritdoc />
    public virtual bool Contains(Point2D<int> p)
    {
        return false;
    }

    /// <inheritdoc />
    public virtual void Draw()
    {
        if (IsDrawn)
        {
            return;
        }

        ModifyBitmap();
        _ = Parent.Children.Add(_image);
        Parent.SizeChanged += Parent_SizeChanged;
        IsDrawn = true;
    }

    private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Scale();
    }

    /// <inheritdoc />
    public virtual void Remove()
    {
        if (!IsDrawn)
        {
            return;
        }

        Parent.Children.Remove(_image);
        Parent.SizeChanged -= Parent_SizeChanged;
        IsDrawn = false;
    }

    /// <inheritdoc />
    public virtual void Scale()
    {
        Clear();

        double xScale = Parent.ActualWidth / IniSize.Width;
        double yScale = Parent.ActualHeight / IniSize.Height;
        for (int i = 0; i < _iniPoints.Count; i++)
        {
            _points[i].X = (int)(_iniPoints[i].X * xScale);
            _points[i].Y = (int)(_iniPoints[i].Y * yScale);
        }
        PointRadius = (int)Math.Round(IniRadius * (xScale + yScale) / 2);

        _bitmap = new WriteableBitmap((int)Parent.ActualWidth, (int)Parent.ActualHeight, dpiX: 96, dpiY: 96, PixelFormats.Pbgra32,
            null);
        _backBuffer = _bitmap.BackBuffer;
        _backBufferStride = _bitmap.BackBufferStride;

        _image.Source = _bitmap;
        _image.Width = _bitmap.Width;
        _image.Height = _bitmap.Height;
        ModifyBitmap();
    }
}
