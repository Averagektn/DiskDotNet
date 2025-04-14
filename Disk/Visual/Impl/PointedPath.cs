using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Disk.Visual.Impl;

public unsafe class PointedPath : IStaticFigure
{
    protected Panel Parent;

    private Color _color;

    private readonly List<Point2D<int>> _points;
    private Image _image;
    private WriteableBitmap _bitmap;
    private IntPtr _backBuffer;
    private int _backBufferStride;
    private Size _lastSize;

    public PointedPath(IEnumerable<Point2D<int>> points, Color color, Panel parent)
    {
        _points = [.. points];
        _color = color;
        Parent = parent;

        _bitmap = new((int)parent.ActualWidth, (int)parent.ActualHeight, dpiX: 96, dpiY: 96, PixelFormats.Pbgra32, null);
        _backBuffer = _bitmap.BackBuffer;
        _backBufferStride = _bitmap.BackBufferStride;
        _lastSize = parent.RenderSize;

        _image = new Image()
        {
            Source = _bitmap,
            Width = _bitmap.Width,
            Height = _bitmap.Height,
        };
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
            uint colorValue = (uint)((_color.A << 24) | (_color.R << 16) | (_color.G << 8) | _color.B);

            for (int i = 0; i < _points.Count; i++)
            {
                int x = _points[i].X;
                int y = _points[i].Y;

                if (x >= 0 && x < _bitmap.PixelWidth && y >= 0 && y < _bitmap.PixelHeight)
                {
                    uint* pixel = (uint*)(_backBuffer + (y * _backBufferStride) + (x * 4));
                    *pixel = colorValue;
                }
            }

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
        }
        finally
        {
            _bitmap.Unlock();
        }
    }

    public void Clear()
    {
        _bitmap.Lock();

        try
        {
            var pixels = (uint*)_backBuffer;
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

    public virtual bool Contains(Point2D<int> p)
    {
        return false;
    }

    public virtual void Draw()
    {
        ModifyBitmap();
        _ = Parent.Children.Add(_image);
        Parent.SizeChanged += Parent_SizeChanged;
    }

    private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Scale();
    }

    public virtual void Remove()
    {
        Parent.Children.Remove(_image);
        Parent.SizeChanged -= Parent_SizeChanged;
    }

    public virtual void Scale()
    {
        Clear();

        double xScale = Parent.ActualWidth / _lastSize.Width;
        double yScale = Parent.ActualHeight / _lastSize.Height;
        _points.ForEach(p =>
        {
            p.X = (int)(p.X * xScale);
            p.Y = (int)(p.Y * yScale);
        });
        _lastSize = Parent.RenderSize;

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
