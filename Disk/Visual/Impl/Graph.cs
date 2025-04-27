using Disk.Calculations.Impl;
using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Disk.Visual.Impl;

/// <summary>
///     Represents a graph that can be drawn and scaled
/// </summary>
public class Graph : IStaticFigure
{
    /// <summary>
    ///     Protects from multiple <see cref="Draw"/> calls
    /// </summary>
    public bool IsDrawn { get; private set; } = false;

    /// <summary>
    ///     The radius of the graph
    /// </summary>
    protected int Radius { get; private set; }

    /// <summary>
    ///     Required for correct positioning
    /// </summary>
    protected readonly Panel Parent;

    /// <summary>
    ///     The polygon used to draw the graph
    /// </summary>
    private readonly Polygon Polygon;

    /// <summary>
    ///     The frequency of data points
    /// </summary>
    private readonly IEnumerable<int> Frequency;

    /// <summary>
    ///     The number of segments in the graph
    /// </summary>
    private readonly int SegmentsNum;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Graph"/> class
    /// </summary>
    /// <param name="points">
    ///     The data points in polar coordinates
    /// </param>
    /// <param name="color">
    ///     The color of the graph
    /// </param>
    /// <param name="parent">
    ///     Element to draw on
    /// </param>
    /// <param name="segmentsNum">
    ///     The number of segments in the graph. Default is 4
    /// </param>
    public Graph(IEnumerable<PolarPoint<float>> points, Brush color, Panel parent, int segmentsNum = 4)
    {
        Parent = parent;
        SegmentsNum = segmentsNum;
        Radius = (int)(Math.Min(Parent.RenderSize.Width, Parent.RenderSize.Height) * 0.9) / 2;

        Frequency = GetFrequency(Classifier<float>.Classify(points.ToList(), segmentsNum));

        Polygon = new()
        {
            Fill = color
        };
    }

    /// <inheritdoc/>
    public void Draw()
    {
        if (IsDrawn)
        {
            return;
        }

        FillPolygon();

        Parent.SizeChanged += Parent_SizeChanged;
        _ = Parent.Children.Add(Polygon);
        IsDrawn = true;
    }

    private void Parent_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        Scale();
    }

    /// <inheritdoc/>
    public void Scale()
    {
        Radius = (int)(Math.Min(Parent.ActualWidth, Parent.ActualHeight) * 0.9) / 2;

        FillPolygon();
    }

    /// <summary>
    ///     Fills the polygon with data points
    /// </summary>
    private void FillPolygon()
    {
        Polygon.Points.Clear();

        var angleStep = 360.0 / SegmentsNum;
        var maxFrequency = Frequency.Max();

        int i = 0;

        for (var angle = angleStep / 2; angle < 360.0; angle += angleStep, i++)
        {
            var radius = Radius * Frequency.ElementAt(i) / (double)maxFrequency;
            var point = new PolarPoint<float>(radius, Math.PI * angle / 180);

            Polygon.Points.Add(new((Parent.RenderSize.Width / 2) + point.X, (Parent.RenderSize.Height / 2) - point.Y));
        }
    }

    /// <summary>
    ///     Gets the frequency of data points in each segment
    /// </summary>
    /// <param name="dataset">
    ///     The dataset of data points
    /// </param>
    /// <returns>
    ///     A list of frequencies of data points
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

    /// <inheritdoc/>
    public void Remove()
    {
        if (!IsDrawn)
        {
            return;
        }

        Parent.Children.Remove(Polygon);
        Parent.SizeChanged -= Parent_SizeChanged;
        IsDrawn = false;
    }

    /// <inheritdoc/>
    public bool Contains(Point2D<int> p)
    {
        return false;
    }
}
