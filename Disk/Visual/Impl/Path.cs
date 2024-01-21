using Disk.Calculations.Impl;
using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Drawing;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl
{
    class Path : IDrawable, IScalable
    {
        private readonly Polyline Polyline;

        private readonly SizeF AngleSize;

        private readonly List<Point2D<float>> Points = [];

        private Converter Converter;

        public Path(IEnumerable<Point2D<float>> points, Size currSize, SizeF angleSize, Brush color)
        {
            AngleSize = angleSize;

            Converter = new(currSize, angleSize);

            Polyline = new Polyline()
            {
                Stroke = color
            };

            foreach (var point in points)
            {
                Polyline.Points.Add(Converter.ToWndCoord(point).ToPoint());
                Points.Add(point);
            }
        }

        public void Draw(IAddChild addChild)
        {
            addChild.AddChild(Polyline);
        }

        public void Scale(Size newSize)
        {
            Converter = new(newSize, AngleSize);

            Polyline.Points.Clear();

            foreach (var point in Points)
            {
                Polyline.Points.Add(Converter.ToWndCoord(point).ToPoint());
            }
        }
    }
}
