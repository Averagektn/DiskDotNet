using Disk.Calculations.Impl;
using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl
{
    /// <summary>
    /// 
    /// </summary>
    class Graph : IDrawable, IScalable
    {
        private Size Size;

        private readonly Polygon Polygon;

        private readonly IEnumerable<int> Frequency;

        private readonly int SegmentsNum;

        private int Radius;
        private Point2D<int> Center;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points">
        /// 
        /// </param>
        /// <param name="currSize">
        /// 
        /// </param>
        /// <param name="color">
        /// 
        /// </param>
        /// <param name="segmentsNum">
        /// 
        /// </param>
        public Graph(IEnumerable<PolarPoint<float>> points, Size currSize, Brush color, Point2D<int> center, int radius, 
            int segmentsNum = 4)
        {
            SegmentsNum = segmentsNum;
            Size = currSize;
            Radius = radius;
            Center = center;

            var l = new List<PolarPoint<float>>();
            foreach (var p in points)
            {
                l.Add(p);
            }

            Frequency = GetFrequency(Classifier<float>.Classify(l, segmentsNum));

            Polygon = new()
            {
                Fill = color
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addChild">
        /// 
        /// </param>
        public void Draw(IAddChild addChild)
        {
            FillPolygon();

            addChild.AddChild(Polygon);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newSize">
        /// 
        /// </param>
        public void Scale(Size newSize)
        {
            var scaleX = newSize.Width / Size.Width;
            var scaleY = newSize.Height / Size.Height;
            var scale = (scaleX + scaleY) / 2;

            Radius = (int)(Radius * scale);

            Center.X = (int)(Center.X * scaleX);
            Center.Y = (int)(Center.Y * scaleY);

            Polygon.Points.Clear();

            FillPolygon();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FillPolygon()
        {
            var angleStep = 360.0 / SegmentsNum;
            var maxFrequency = Frequency.Max();

            int i = 0;

            for (var angle = angleStep / 2; angle < 360.0; angle += angleStep, i++)
            {
                var radius = Radius * (Frequency.ElementAt(i) + 1) / (double)maxFrequency;

                var point = new PolarPoint<float>(radius, Math.PI * angle / 180);

                Polygon.Points.Add(new(point.X + Center.X, Center.Y - point.Y));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        private static List<int> GetFrequency(IEnumerable<IEnumerable<PolarPoint<float>>> dataset)
        {
            var res = new List<int>(dataset.Count());

            foreach (var points in dataset)
            {
                res.Add(points.Count());
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(UIElementCollection collection)
        {
            collection.Remove(Polygon);
        }
    }
}
