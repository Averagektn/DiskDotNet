using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Db.Context;
using Disk.Entities;
using Disk.Service.Implementation;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using Emgu.CV;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Localization = Disk.Properties.Langs.AttemptResult.AttemptResultLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class AttemptResultViewModel(NavigationStore navigationStore, DiskContext database) : PopupViewModel
{
    private long _attemptId;
    public required long AttemptId
    {
        get => _attemptId;
        set
        {
            _attemptId = value;
            _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                CurrentAttempt = await database.Attempts
                    .Where(s => s.Id == value)
                    .Include(s => s.SessionNavigation)
                    .Include(s => s.SessionNavigation.MapNavigation)
                    .Include(s => s.PathToTargets)
                    .Include(s => s.PathInTargets)
                    .Include(s => s.AttemptResult)
                    .FirstAsync());
        }
    }

    private Attempt _currentAttempt = null!;
    public required Attempt CurrentAttempt
    {
        get => _currentAttempt;
        set
        {
            _currentAttempt = value;

            TargetCenters = JsonConvert
                .DeserializeObject<List<Point2D<float>>>(CurrentAttempt.SessionNavigation.MapNavigation.CoordinatesJson)!;
            PathsToTargets = CurrentAttempt.PathToTargets
                .Select(ptt => JsonConvert.DeserializeObject<List<Point2D<float>>>(ptt.CoordinatesJson)!)
                .ToList() ?? [];
            PathsInTargets = CurrentAttempt.PathInTargets
                .Select(pit => JsonConvert.DeserializeObject<List<Point2D<float>>>(pit.CoordinatesJson)!)
                .ToList() ?? [];

            IsPathChecked = true;
            SelectedIndex = 0;
            OnPropertyChanged(nameof(UserCenter));
            OnPropertyChanged(nameof(TargetCenter));

            NewItemSelectedCommand.Execute(null);
        }
    }

    public Point2D<int> UserCenter
    {
        get
        {
            if (PathsInTargets.Count == 0 || SelectedIndex < 0)
            {
                return new(0, 0);
            }
            return Converter.ToWndCoord(PathsToTargets[SelectedIndex][0]);
        }
    }
    public Point2D<int> TargetCenter
    {
        get
        {
            if (TargetCenters.Count == 0 || SelectedIndex < 0)
            {
                return new(0, 0);
            }
            return Converter.ToWndCoord(TargetCenters[SelectedIndex]);
        }
    }
    public static Settings Settings => Settings.Default;

    private List<List<Point2D<float>>> PathsToTargets { get; set; } = [];
    private List<List<Point2D<float>>> PathsInTargets { get; set; } = [];

    public List<Point2D<float>> CurrPathToTarget
    {
        get
        {
            if (SelectedIndex < 0 || SelectedIndex > PathsToTargets.Count)
            {
                return [];
            }
            return PathsToTargets[SelectedIndex];
        }
    }
    public List<Point2D<float>> CurrPathInTarget
    {
        get
        {
            if (SelectedIndex < 0 || SelectedIndex > PathsInTargets.Count)
            {
                return [];
            }
            return PathsInTargets[SelectedIndex];
        }
    }

    public ObservableCollection<string> Indices { get; set; } = [];
    public Converter Converter { get; set; } = DrawableFabric.GetIniConverter();
    public bool ShowPathInTarget { get; set; }
    public bool ShowPathToTarget { get; set; }

    public IEnumerable<(bool IsNewTarget, Point2D<float> Point)> FullPath
    {
        get
        {
            Point2D<float> lastPoint = null!;

            for (int i = SelectedIndex; i < TargetCenters.Count; i++)
            {
                var ptt = JsonConvert
                    .DeserializeObject<List<Point2D<float>>>(CurrentAttempt.PathToTargets.ElementAt(i).CoordinatesJson)!;
                foreach (var point in ptt)
                {
                    yield return (false, point);
                }

                var pit = JsonConvert
                    .DeserializeObject<List<Point2D<float>>>(CurrentAttempt.PathInTargets.ElementAt(i).CoordinatesJson)!;
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
            {Localization.StandartDeviation} X: {CurrentAttempt.AttemptResult?.DeviationX:F2}
            {Localization.StandartDeviation} Y: {CurrentAttempt.AttemptResult?.DeviationY:F2}
            {Localization.MathExp} X: {CurrentAttempt.AttemptResult?.MathExpX:F2}
            {Localization.MathExp} Y: {CurrentAttempt.AttemptResult?.MathExpY:F2}
            {Localization.AverageSpeed}: {CurrentAttempt.PathToTargets.ElementAt(SelectedIndex).AverageSpeed:F2}
            {Localization.ApproachSpeed}: {CurrentAttempt.PathToTargets.ElementAt(SelectedIndex).ApproachSpeed:F2}
            {Localization.Time}: {CurrentAttempt.PathToTargets.ElementAt(SelectedIndex).Time:F2}
            {Localization.Accuracy}: {CurrentAttempt.PathInTargets.ElementAt(SelectedIndex).Accuracy:F2}  
            """);

    public void FillTargetsComboBox()
    {
        for (int i = 0; i < TargetCenters.Count; i++)
        {
            Indices.Add($"{Localization.Target} {i + 1}");
        }
    }

    // remove
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
                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                    await ShowPopup(header: "", message: Localization.NoContentForPathError));
                return [];
            }

            var res = new List<IStaticFigure>();
            //var pathToTarget = new Visual.Impl.Path(PathsToTargets[SelectedIndex], Converter, Brushes.Green, canvas);
            //var pathInTarget = new Visual.Impl.Path(PathsInTargets[SelectedIndex], Converter, Brushes.Blue, canvas);
            var c = DrawableFabric.GetIniConverter();
            var pointsToTarget = new PointedPath(PathsToTargets[SelectedIndex].Select(Converter.ToWndCoord), Colors.Green, canvas);
            var pointsInTarget = new PointedPath(PathsInTargets[SelectedIndex].Select(Converter.ToWndCoord), Colors.Blue, canvas);

            Log.Information("Created Path");

            if (ShowPathToTarget)
            {
                //res.Add(pathToTarget);
                res.Add(pointsToTarget);
            }
            if (ShowPathInTarget)
            {
                //res.Add(pathInTarget);
                res.Add(pointsInTarget);
            }

            return res;
        }

        return [];
    }

    private Graph GetGraph(Canvas canvas)
    {
        if (PathsInTargets.Count <= SelectedIndex || PathsInTargets[SelectedIndex].Count == 0)
        {
            _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                await ShowPopup(header: "", message: Localization.NoContentForDiagramError));
            return new Graph([], Brushes.LightGreen, canvas, 8);
        }

        var target = new ProgressTarget
        (
            center: new(0, 0),
            radius: CurrentAttempt.TargetRadius,
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

        Log.Information("Created Graph");

        return new Graph(dataset, Brushes.LightGreen, canvas, 8);
    }

    public List<Point2D<float>>? GetConvexHull()
    {
        if (SelectedIndex < 0)
        {
            return null;
        }

        var coords = PathsToTargets[SelectedIndex].Concat(PathsInTargets[SelectedIndex]).ToList();
        var points = new List<PointF>(coords.Count);
        coords.ForEach(coord => points.Add(coord.ToPointF()));

        var convexhull = CvInvoke.ConvexHull([.. points], true);
        var result = new List<Point2D<float>>(convexhull.Length);
        foreach (var item in convexhull)
        {
            result.Add(new Point2D<float>(item.X, item.Y));
        }

        return result;
    }
}
