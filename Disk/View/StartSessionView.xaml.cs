using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.ViewModel;
using Disk.Visual.Impl;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View;

public partial class StartSessionView : UserControl
{
    private StartSessionViewModel? ViewModel => (StartSessionViewModel)DataContext;

    private static int IniWidth = Settings.Default.IniScreenWidth;
    private static int IniHeight = Settings.Default.IniScreenHeight;
    private static float AngleWidth => Settings.Default.XMaxAngle * 2;
    private static float AngleHeight => Settings.Default.YMaxAngle * 2;

    private Converter _converter = null!;
    private readonly List<NumberedTarget> _targets = [];

    public StartSessionView()
    {
        InitializeComponent();

        PaintArea.Loaded += OnPaintAreaLoaded;
        PaintArea.SizeChanged += OnPaintAreaSizeChanged;
    }

    private void OnPaintAreaSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _targets.ForEach(target => target.Scale());

        FullRadiusEllipse.RadiusX = e.NewSize.Width / 2;
        FullRadiusEllipse.RadiusY = e.NewSize.Height / 2;
        FullRadiusEllipse.Center = new(e.NewSize.Width / 2, e.NewSize.Height / 2);
    }

    private void OnPaintAreaLoaded(object sender, RoutedEventArgs e)
    {
        _converter = new Converter((int)PaintArea.ActualWidth, (int)PaintArea.ActualHeight, AngleWidth, AngleHeight);
        IniWidth = (int)PaintArea.ActualWidth;
        IniHeight = (int)PaintArea.ActualHeight;
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var map = ViewModel?.SelectedMap;
        if (map is null)
        {
            return;
        }

        _targets.ForEach(target => target.Remove());
        _targets.Clear();
        var coords = JsonConvert.DeserializeObject<List<Point2D<float>>>(map.CoordinatesJson) ?? [];
        coords.ForEach(point =>
        {
            var wnd = _converter.ToWndCoord(point);
            var target = GetIniCoordTarget(wnd.X, wnd.Y);
            target.Scale();
            target.Draw();
            target.HideAngles();
            _targets.Add(target);
        });
    }

    private NumberedTarget GetIniCoordTarget(int actualX, int actualY)
    {
        return new(
            center: new Point2D<int>
            (
                actualX,
                actualY
            ),
            radius: Settings.Default.IniTargetRadius,
            parent: PaintArea,
            number: _targets.Count + 1,
            iniSize: new(IniWidth, IniHeight),
            converter: _converter!
        );
    }
}
