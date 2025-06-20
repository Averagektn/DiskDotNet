﻿using System.Windows;
using System.Windows.Controls;

using Disk.Calculations.Implementations.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Visual.Implementations;

using Newtonsoft.Json;

using Settings = Disk.Properties.Config.Config;

namespace Disk.Views;

public partial class MapPreviewView : UserControl
{
    public static readonly DependencyProperty MapProperty =
        DependencyProperty.Register(
            nameof(Map),
            typeof(Map),
            typeof(MapPreviewView),
            new PropertyMetadata(null, OnMapChanged)
        );

    public Map? Map
    {
        get => (Map?)GetValue(MapProperty);
        set => SetValue(MapProperty, value);
    }

    private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (MapPreviewView)d;
        control.RedrawMap();
    }

    private Converter? _converter;
    private readonly List<NumberedTarget> _targets = [];
    private readonly Converter IniConverter;
    private static int IniWidth => Settings.Default.IniScreenWidth;
    private static int IniHeight => Settings.Default.IniScreenHeight;
    private static float AngleWidth => Settings.Default.XMaxAngle * 2;
    private static float AngleHeight => Settings.Default.YMaxAngle * 2;

    public MapPreviewView()
    {
        InitializeComponent();

        IniConverter = new(IniWidth, IniHeight, AngleWidth, AngleHeight);
        PaintArea.SizeChanged += OnPaintAreaSizeChanged;
    }

    private void OnPaintAreaSizeChanged(object sender, SizeChangedEventArgs e)
    {
        FullRadiusEllipse.RadiusX = e.NewSize.Width / 2;
        FullRadiusEllipse.RadiusY = e.NewSize.Height / 2;
        FullRadiusEllipse.Center = new(e.NewSize.Width / 2, e.NewSize.Height / 2);

        _converter ??= new Converter((int)e.NewSize.Width, (int)e.NewSize.Height, AngleWidth, AngleHeight);
        _converter.Scale(e.NewSize);
    }

    private void RedrawMap()
    {
        if (Map is null)
        {
            return;
        }

        _targets.ForEach(target => target.Remove());
        _targets.Clear();
        List<Point2D<float>> coords = JsonConvert.DeserializeObject<List<Point2D<float>>>(Map.CoordinatesJson) ?? [];
        coords.ForEach(point =>
        {
            Point2D<int> wnd = IniConverter.ToWndCoord(point);
            NumberedTarget target = GetIniCoordTarget(wnd.X, wnd.Y);
            target.Draw();
            target.HideAngles();
            _targets.Add(target);
        });
    }

    private NumberedTarget GetIniCoordTarget(int actualX, int actualY)
    {
        _converter ??= new Converter((int)PaintArea.ActualWidth, (int)PaintArea.ActualHeight, AngleWidth, AngleHeight);

        return new(
            center: new Point2D<int>
            (
                actualX,
                actualY
            ),
            radius: Settings.Default.IniTargetRadius * 5,
            parent: PaintArea,
            number: _targets.Count + 1,
            iniSize: new(IniWidth, IniHeight),
            converter: _converter
        );
    }
}
