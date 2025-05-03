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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Localization = Disk.Properties.Langs.AttemptResult.AttemptResultLocalization;
using Settings = Disk.Properties.Config.Config;
using Size = System.Windows.Size;

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

            CurrentAttempt = database.Attempts
                .Where(s => s.Id == value)
                .Include(s => s.SessionNavigation)
                .Include(s => s.SessionNavigation.MapNavigation)
                .Include(s => s.PathToTargets)
                .Include(s => s.PathInTargets)
                .Include(s => s.AttemptResult)
                .First();
        }
    }

    private Attempt _currentAttempt = null!;
    public required Attempt CurrentAttempt
    {
        get => _currentAttempt;
        set
        {
            _currentAttempt = value;

            PathsToTargets = CurrentAttempt.PathToTargets
                .Select(ptt => JsonConvert.DeserializeObject<List<Point2D<float>>>(ptt.CoordinatesJson)!)
                .ToList() ?? [];
            PathsInTargets = CurrentAttempt.PathInTargets
                .Select(pit => JsonConvert.DeserializeObject<List<Point2D<float>>>(pit.CoordinatesJson)!)
                .ToList() ?? [];

            PointsCount = PathsToTargets.Sum(p => p.Count) + PathsInTargets.Sum(p => p.Count);

            TargetCenters = [.. JsonConvert
                .DeserializeObject<List<Point2D<float>>>(CurrentAttempt.SessionNavigation.MapNavigation.CoordinatesJson)!
                .Take(int.Max(PathsInTargets.Count, PathsToTargets.Count))];

            SelectedIndex = 0;
            OnPropertyChanged(nameof(CursorCenter));
            OnPropertyChanged(nameof(TargetCenter));

            NewItemSelectedCommand.Execute(null);
        }
    }

    private int _currentPointId;
    public int CurrentPointId
    {
        get => _currentPointId;
        set
        {
            if (value >= PointsCount)
            {
                return;
            }

            _ = SetProperty(ref _currentPointId, value);

            int newIndex = 0;
            int newPointId = value;
            bool isPathToTarget = true;

            while (newPointId >= 0)
            {
                if (isPathToTarget)
                {
                    newPointId -= PathsToTargets[newIndex].Count;
                    isPathToTarget = false;
                }
                else
                {
                    newPointId -= PathsInTargets[newIndex].Count;
                    isPathToTarget = true;
                    if (newPointId >= 0)
                    {
                        newIndex++;
                    }
                }
            }

            IsPathToTarget = !isPathToTarget;

            if (IsPathToTarget)
            {
                SelectedPathPointId = newPointId + PathsToTargets[newIndex].Count;
            }
            else
            {
                SelectedPathPointId = newPointId + PathsInTargets[newIndex].Count;
            }

            SelectedIndex = newIndex;
        }
    }

    private int _selectedPathPointId;
    public int SelectedPathPointId
    {
        get => _selectedPathPointId;
        set
        {
            _ = SetProperty(ref _selectedPathPointId, value);
        }
    }

    private int _pointsCount;
    public int PointsCount { get => _pointsCount; set => SetProperty(ref _pointsCount, value); }

    public Point2D<int> CursorCenter
    {
        get
        {
            if (IsPathToTarget)
            {
                return Converter.ToWndCoord(CurrPathToTarget[SelectedPathPointId]);
            }
            return Converter.ToWndCoord(CurrPathInTarget[SelectedPathPointId]);
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
            if (SelectedIndex < 0 || SelectedIndex >= PathsToTargets.Count)
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
            if (SelectedIndex < 0 || SelectedIndex >= PathsInTargets.Count)
            {
                return [];
            }
            return PathsInTargets[SelectedIndex];
        }
    }

    public ObservableCollection<string> Indices { get; set; } = [];
    public Converter Converter { get; set; } = DrawableFabric.GetIniConverter();
    public bool IsPathToTarget { get; private set; }

    public IEnumerable<Point2D<float>> FullPath
    {
        get
        {
            bool isLastPoint = false;

            while (!isLastPoint)
            {
                if (IsPathToTarget)
                {
                    yield return CurrPathToTarget[SelectedPathPointId];
                }
                else
                {
                    yield return CurrPathInTarget[SelectedPathPointId];
                }
                CurrentPointId++;
                isLastPoint = CurrentPointId == PointsCount - 1;
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

    private int _selectedIndex = -1;
    public int SelectedIndex { get => _selectedIndex; set => SetProperty(ref _selectedIndex, value); }

    public bool IsRepeatEnabled => !IsStopEnabled;

    private bool _isStopEnabled;
    public bool IsStopEnabled
    {
        get => _isStopEnabled;
        set
        {
            _ = SetProperty(ref _isStopEnabled, value);
            OnPropertyChanged(nameof(IsRepeatEnabled));
        }
    }

    public ICommand NavigateBackCommand => new Command(_ => navigationStore.Close());
    public ICommand NewItemSelectedCommand => new Command(_ =>
    {
        if (SelectedIndex >= 0 && CurrentAttempt.PathToTargets.Count > SelectedIndex && CurrentAttempt.PathInTargets.Count > SelectedIndex)
        {
            int pathToTargetPointsCount = PathsToTargets.Take(SelectedIndex).Sum(p => p.Count);
            int pathInTargetPointsCount = PathsInTargets.Take(SelectedIndex).Sum(p => p.Count);
            CurrentPointId = pathToTargetPointsCount + pathInTargetPointsCount;

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
                """;
        }
    });

    public void FillTargetsComboBox()
    {
        for (int i = 0; i < TargetCenters.Count; i++)
        {
            Indices.Add($"{Localization.Target} {i + 1}");
        }
    }

    public (IStaticFigure? PathToTarget, IStaticFigure? PathInTarget) GetPath(Panel parent)
    {
        if (SelectedIndex == -1)
        {
            return (null, null);
        }

        if (PathsToTargets.Count <= SelectedIndex || PathsToTargets[SelectedIndex].Count == 0)
        {
            _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                await ShowPopup(header: "", message: Localization.NoContentForPathError));

            return (null, null);
        }

        var c = DrawableFabric.GetIniConverter();
        var iniSize = new Size(Settings.IniScreenWidth, Settings.IniScreenHeight);

        IStaticFigure pointsToTarget = new PointedPath(CurrPathToTarget.Select(c.ToWndCoord), Colors.Green, parent, iniSize);
        pointsToTarget.Scale();

        IStaticFigure pointsInTarget = new PointedPath(CurrPathInTarget.Select(c.ToWndCoord), Colors.Blue, parent, iniSize);
        pointsInTarget.Scale();

        return (pointsToTarget, pointsInTarget);
    }

    public IStaticFigure? GetGraph(Panel parent)
    {
        if (SelectedIndex == -1)
        {
            return null;
        }

        if (PathsInTargets.Count <= SelectedIndex || PathsInTargets[SelectedIndex].Count == 0)
        {
            _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                await ShowPopup(header: "", message: Localization.NoContentForDiagramError));
            return null;
        }

        var target = new ProgressTarget
        (
            center: new(0, 0),
            radius: CurrentAttempt.TargetRadius,
            parent: parent,
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

        return new Graph(dataset, Brushes.LightGreen, parent, 8);
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
