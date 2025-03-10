﻿using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Service.Implementation;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Localization = Disk.Properties.Langs.SessionResult.SessionResultLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class SessionResultViewModel(NavigationStore navigationStore) : ObserverViewModel
{
    private List<List<Point2D<float>>> PathsToTargets { get; set; } = [];
    private List<List<Point2D<float>>> PathsInTargets { get; set; } = [];
    public Point2D<int> UserCenter => Converter.ToWndCoord(PathsToTargets[SelectedIndex][0]);
    public Point2D<int> TargetCenter => Converter.ToWndCoord(TargetCenters[SelectedIndex]);
    public ObservableCollection<string> Indices { get; set; } = [];
    public Converter Converter { get; set; } = DrawableFabric.GetIniConverter();
    public Settings Settings => Settings.Default;

    private Session _currentSession = null!;

    public bool ShowPathInTarget { get; set; }
    public bool ShowPathToTarget { get; set; }

    public required Session CurrentSession
    {
        get => _currentSession;
        set
        {
            _currentSession = value;
            TargetCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(CurrentSession.AppointmentNavigation.MapNavigation.CoordinatesJson)!;
            PathsToTargets = CurrentSession.PathToTargets
                .Select(ptt => JsonConvert.DeserializeObject<List<Point2D<float>>>(ptt.CoordinatesJson)!)
                .ToList() ?? [];
            PathsInTargets = CurrentSession.PathInTargets
                .Select(pit => JsonConvert.DeserializeObject<List<Point2D<float>>>(pit.CoordinatesJson)!)
                .ToList() ?? [];

            IsPathChecked = true;
            SelectedIndex = 0;

            NewItemSelectedCommand.Execute(null);
        }
    }
    public IEnumerable<(bool IsNewTarget, Point2D<float> Point)> FullPath
    {
        get
        {
            Point2D<float> lastPoint = null!;

            for (int i = SelectedIndex; i < TargetCenters.Count; i++)
            {
                var ptt = JsonConvert
                    .DeserializeObject<List<Point2D<float>>>(CurrentSession.PathToTargets.ElementAt(i).CoordinatesJson)!;
                foreach (var point in ptt)
                {
                    yield return (false, point);
                }

                var pit = JsonConvert
                    .DeserializeObject<List<Point2D<float>>>(CurrentSession.PathInTargets.ElementAt(i).CoordinatesJson)!;
                foreach (var point in pit)
                {
                    lastPoint = point;
                    yield return (false, point);
                }

                yield return (true, lastPoint);
            }
        }
    }

    private List<Point2D<float>> _targetCenters = [];
    public List<Point2D<float>> TargetCenters
    {
        get => _targetCenters;
        set
        {
            _targetCenters = value;
            FillTargetsComboBox();
        }
    }

    private string _message = string.Empty;
    public string Message { get => _message; set => SetProperty(ref _message, value); }

    private bool _isDiagramChecked = false;
    public bool IsDiagramChecked { get => _isDiagramChecked; set => SetProperty(ref _isDiagramChecked, value); }

    private bool _isPathChecked = false;
    public bool IsPathChecked { get => _isPathChecked; set => SetProperty(ref _isPathChecked, value); }

    private int _selectedIndex = -1;
    public int SelectedIndex { get => _selectedIndex; set => SetProperty(ref _selectedIndex, value); }

    private bool _isStopEnabled;
    public bool IsStopEnabled { get => _isStopEnabled; set => SetProperty(ref _isStopEnabled, value); }

    public ICommand NavigateBackCommand => new Command(_ => navigationStore.Close());
    public ICommand NewItemSelectedCommand => new Command(_ =>
        Message =
        $"""
            {Localization.StandartDeviation} X: {CurrentSession.SessionResult?.DeviationX:F2}
            {Localization.StandartDeviation} Y: {CurrentSession.SessionResult?.DeviationY:F2}
            {Localization.MathExp} X: {CurrentSession.SessionResult?.MathExpX:F2}
            {Localization.MathExp} Y: {CurrentSession.SessionResult?.MathExpY:F2}
            {Localization.AverageSpeed}: {CurrentSession.PathToTargets.ElementAt(SelectedIndex).AverageSpeed:F2}
            {Localization.ApproachSpeed}: {CurrentSession.PathToTargets.ElementAt(SelectedIndex).ApproachSpeed:F2}
            {Localization.Time}: {CurrentSession.PathToTargets.ElementAt(SelectedIndex).Time:F2}
            {Localization.Precision}: {CurrentSession.PathInTargets.ElementAt(SelectedIndex).Precision:F2}  
            """);

    public void FillTargetsComboBox()
    {
        for (int i = 0; i < TargetCenters.Count; i++)
        {
            Indices.Add($"{Localization.Target} {i + 1}");
        }
    }

    public List<IStaticFigure> GetPathAndRose(Canvas canvas)
    {
        if (SelectedIndex == -1)
        {
            return [];
        }

        if (IsDiagramChecked)
        {
            return [GetGraph(canvas)];
        }
        else if (IsPathChecked)
        {
            if (PathsToTargets.Count <= SelectedIndex || PathsToTargets[SelectedIndex].Count == 0)
            {
                _ = MessageBox.Show(Localization.NoContentForPathError);
                return [];
            }

            var res = new List<IStaticFigure>();
            var pathToTarget = new Path(PathsToTargets[SelectedIndex], Converter, new SolidColorBrush(Colors.Green), canvas);
            var pathInTarget = new Path(PathsInTargets[SelectedIndex], Converter, new SolidColorBrush(Colors.Blue), canvas);

            if (ShowPathToTarget)
            {
                res.Add(pathToTarget);
            }
            if (ShowPathInTarget)
            {
                res.Add(pathInTarget);
            }

            return res;
        }

        return [];
    }

    private Graph GetGraph(Canvas canvas)
    {
        if (PathsInTargets.Count <= SelectedIndex || PathsInTargets[SelectedIndex].Count == 0)
        {
            _ = MessageBox.Show(Localization.NoContentForDiagramError);
            return new Graph([], Brushes.LightGreen, canvas, 8);
        }

        var target = new ProgressTarget
        (
            center: new(0, 0), 
            radius: CurrentSession.TargetRadius,
            parent: canvas,
            hp: 0,
            iniSize: new(Settings.IniScreenWidth, Settings.IniScreenHeight)
        );
        target.Scale();

        var angRadius = (Converter.ToAngleX_FromWnd(target.Radius) + Converter.ToAngleY_FromWnd(target.Radius)) / 2;

        var angCenter = Converter.ToAngle_FromWnd(Converter.ToWndCoord(TargetCenters[SelectedIndex]));
        var dataset =
            PathsInTargets[SelectedIndex]
            .Select(p => new PolarPoint<float>(p.X - angCenter.X, p.Y - angCenter.Y))
            // Cutoff points in target
            //.Where(p => Math.Abs(p.X) > angRadius && Math.Abs(p.Y) > angRadius)
            .ToList();

        return new Graph(dataset, Brushes.LightGreen, canvas, 8);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();

        Application.Current.MainWindow.WindowState = WindowState.Normal;
    }
}
