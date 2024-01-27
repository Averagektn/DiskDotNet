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
        public Graph(IEnumerable<PolarPoint<float>> points, Size currSize, Brush color, int segmentsNum = 4)
        {
            SegmentsNum = segmentsNum;

            Size = currSize;

            Radius = (int)(Math.Min(currSize.Width / 2, currSize.Height / 2) * 0.9);

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
            Size = newSize;

            Radius = (int)(Math.Min(newSize.Width / 2, newSize.Height / 2) * 0.9);

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

                Polygon.Points.Add(new(point.X + Size.Width / 2, Size.Height / 2 - point.Y));
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
