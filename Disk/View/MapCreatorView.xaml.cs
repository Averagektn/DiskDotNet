using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.ViewModel;
using Disk.Visual.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View;

public partial class MapCreatorView : UserControl
{
    private static readonly Converter IniConverter = new(IniWidth, IniHeight, AngleWidth, AngleHeight);
    private static int IniWidth => Settings.Default.IniScreenWidth;
    private static int IniHeight => Settings.Default.IniScreenHeight;
    private static float AngleWidth => Settings.Default.XMaxAngle * 2;
    private static float AngleHeight => Settings.Default.YMaxAngle * 2;

    private MapCreatorViewModel? ViewModel => DataContext as MapCreatorViewModel;
    private NumberedTarget? _selectedTarget = null;
    private readonly List<NumberedTarget> _targets = [];
    private Converter? _converter = null;

    public MapCreatorView()
    {
        InitializeComponent();

        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseLeftButtonUp += OnMouseLeftButtonUp;
        MouseRightButtonDown += OnMouseRightButtonDown;
        MouseDoubleClick += OnMouseDoubleClick;
        MouseMove += OnMouseMove;
        Loaded += OnLoaded;
        LayoutUpdated += OnLayoutUpdated;

        PaintArea.SizeChanged += PaintAreaSizeChanged;
    }

    private void PaintAreaSizeChanged(object sender, SizeChangedEventArgs e)
    {
        int newWidth = (int)e.NewSize.Width;
        int newHeight = (int)e.NewSize.Height;

        OneThirdRadiusEllipse.RadiusX = newWidth / 6;
        OneThirdRadiusEllipse.RadiusY = newHeight / 6;
        OneThirdRadiusEllipse.Center = new(newWidth / 2, newHeight / 2);

        TwoThirdRadiusEllipse.RadiusX = newWidth / 3;
        TwoThirdRadiusEllipse.RadiusY = newHeight / 3;
        TwoThirdRadiusEllipse.Center = new(newWidth / 2, newHeight / 2);

        FullRadiusEllipse.RadiusX = newWidth / 2;
        FullRadiusEllipse.RadiusY = newHeight / 2;
        FullRadiusEllipse.Center = new(newWidth / 2, newHeight / 2);

        _converter ??= new Converter(newWidth, newHeight, AngleWidth, AngleHeight);
        _converter?.Scale(new(newWidth, newHeight));

        Canvas.SetLeft(MaxX, newWidth - 2 - MaxX.ActualWidth);
        Canvas.SetTop(MaxX, (newHeight / 2) + 2);

        Canvas.SetLeft(MaxY, (newWidth / 2) + 2);
        Canvas.SetTop(MaxY, 2);
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        Canvas.SetLeft(MaxX, PaintArea.ActualWidth - 2 - MaxX.ActualWidth);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        MaxX.Text = $"X:{Settings.Default.XMaxAngle:f1}";
        MaxY.Text = $"Y: {Settings.Default.YMaxAngle:f1}";
    }

    private bool _isMoveTriggered = false;
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isMoveTriggered = false;
    }
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var mousePos = e.GetPosition(sender as UIElement);
        var x = (int)mousePos.X;
        var y = (int)mousePos.Y;

        var prevTarget = _selectedTarget;
        _selectedTarget = _targets.FindLast(target => target.Contains(new Point2D<int>(x, y)));

        prevTarget?.HideAngles();
        _selectedTarget?.ShowAngles();

        _isMoveTriggered = _selectedTarget is not null;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _isMoveTriggered)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;
            if (FullRadiusEllipse.FillContains(mousePos))
            {
                var clickPoint = new Point2D<int>(x, y);
                _selectedTarget?.Move(clickPoint);
            }
            else
            {
                var center = new Point2D<int>((int)FullRadiusEllipse.Bounds.Width / 2, (int)FullRadiusEllipse.Bounds.Height / 2);
                var radiusX = FullRadiusEllipse.Bounds.Width / 2;
                var radiusY = FullRadiusEllipse.Bounds.Height / 2;

                double normalizedX = (x - center.X) / radiusX;
                double normalizedY = (y - center.Y) / radiusY;

                double length = Math.Sqrt((normalizedX * normalizedX) + (normalizedY * normalizedY));

                double scale = 1 / length;
                int nearestX = (int)(center.X + (normalizedX * radiusX * scale));
                int nearestY = (int)(center.Y + (normalizedY * radiusY * scale));

                _selectedTarget?.Move(new(nearestX, nearestY));
            }
        }
    }

    private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var mousePos = e.GetPosition(sender as UIElement);
        var x = (int)mousePos.X;
        var y = (int)mousePos.Y;

        var target = _targets.FindLast(target => target.Contains(new(x, y)));
        if (target is not null)
        {
            _ = _targets.Remove(target);
            target.Remove();

            for (int i = 0; i < _targets.Count; i++)
            {
                _targets[i].UpdateNumber(i + 1);
            }
        }
    }

    private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var mousePos = e.GetPosition(sender as UIElement);
        var fillContains = FullRadiusEllipse.FillContains(mousePos);

        if (e.ChangedButton == MouseButton.Left && fillContains)
        {
            var target = GetIniCoordTarget(mousePos.X, mousePos.Y);
            target.Draw();
            _targets.Add(target);
        }
    }

    private NumberedTarget GetIniCoordTarget(double actualX, double actualY)
    {
        _converter ??= new Converter((int)PaintArea.ActualWidth, (int)PaintArea.ActualHeight, AngleWidth, AngleHeight);
        return new(
            center: new Point2D<int>
            (
                (int)(actualX / PaintArea.ActualWidth * IniWidth),
                (int)(actualY / PaintArea.ActualHeight * IniHeight)
            ),
            radius: Settings.Default.IniTargetRadius,
            parent: PaintArea,
            number: _targets.Count + 1,
            iniSize: new(IniWidth, IniHeight),
            converter: _converter
        );
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.SaveMap(_targets);
    }
}
