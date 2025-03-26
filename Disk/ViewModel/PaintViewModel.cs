using Disk.Calculations.Impl;
using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Service.Implementation;
using Disk.Stores;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Windows;
using FilePath = System.IO.Path;
using Localization = Disk.Properties.Langs.PaintWindow.PaintWindowLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class PaintViewModel : PopupViewModel
{
    private long _attemptId;
    public required long AttemptId
    {
        get => _attemptId;
        set
        {
            _attemptId = value;
            _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                CurrentAttempt = await _database.Attempts
                    .Where(s => s.Id == value)
                    .Include(s => s.SessionNavigation)
                    .Include(s => s.SessionNavigation.MapNavigation)
                    .FirstAsync());
        }
    }


    private Attempt _currentAttempt = null!;
    public Attempt CurrentAttempt
    {
        get => _currentAttempt;
        set
        {
            _currentAttempt = value;
            TargetCenters = JsonConvert
                .DeserializeObject<List<Point2D<float>>>(_currentAttempt.SessionNavigation.MapNavigation.CoordinatesJson) ?? [];
        }
    }

    // Scale
    public Converter Converter { get; set; }

    // Get only
    public Point2D<float>? NextTargetCenter => TargetCenters.Count <= TargetId ? null : TargetCenters[TargetId++];
    private static Settings Settings => Settings.Default;
    private string UsrAngLog => $"{CurrentAttempt.LogFilePath}{FilePath.DirectorySeparatorChar}{Settings.UserLogFileName}";
    public bool IsPathToTarget => PathToTargetStopwatch?.IsRunning ?? false;

    // Disposable
    private readonly Thread DiskNetworkThread;

    // Attempts datasets
    public List<Point2D<float>> TargetCenters { get; set; } = [];

    public List<Point2D<float>> FullPath = [];
    public List<List<Point2D<float>>> PathsToTargets = [[]];
    public List<List<Point2D<float>>> PathsInTargets = [];

    // Changing
    public Point3D<float>? CurrentPos;
    public bool IsGame = true;
    public int TargetId { get; set; }

    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            OnPropertyChanged(nameof(ScoreString));
            _ = SetProperty(ref _score, value);
        }
    }

    private Stopwatch? PathToTargetStopwatch;
    private bool connectionFailed;

    // binding
    public string ScoreString => $"{Localization.Score}: {Score}";

    private bool _isStopEnabled = true;
    public bool IsStopEnabled { get => _isStopEnabled; set => SetProperty(ref _isStopEnabled, value); }

    // DI
    private readonly NavigationStore _navigationStore;
    private readonly DiskContext _database;

    public PaintViewModel(NavigationStore navigationStore, DiskContext database)
    {
        DiskNetworkThread = new(ReceiveUserPos);

        Converter = DrawableFabric.GetIniConverter();

        _navigationStore = navigationStore;
        _database = database;
    }

    public void StartReceiving()
    {
        DiskNetworkThread.Start();
        PathToTargetStopwatch = Stopwatch.StartNew();
    }

    private void ReceiveUserPos()
    {
        try
        {
            using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.Port);

            while (IsGame)
            {
                CurrentPos = con.GetXYZ();
            }
        }
        catch
        {
            connectionFailed = true;

            _ = Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await ShowPopup(header: Localization.ConnectionLost, message: "");
                await SaveAttemptResultAsync();
            });
        }
    }

    public async Task SaveAttemptResultAsync()
    {
        if (PathsInTargets.Count != 0)
        {
            var mx = Calculator2D.MathExp(FullPath);
            var deviation = Calculator2D.StandartDeviation(FullPath);

            var sres = new AttemptResult()
            {
                Id = CurrentAttempt.Id,
                MathExpX = mx.XDbl,
                MathExpY = mx.YDbl,
                DeviationX = deviation.XDbl,
                DeviationY = deviation.YDbl,
                Score = Score,
            };

            _ = await _database.AttemptResults.AddAsync(sres);
        }
        else
        {
            _ = _database.Attempts.Remove(CurrentAttempt);
        }
        _ = await _database.SaveChangesAsync();

        using (var logger = Logger.GetLogger(UsrAngLog))
        {
            foreach (var point in FullPath)
            {
                logger.LogLn(point);
            }
        }

        IsStopEnabled = false;
        IsGame = false;
        DiskNetworkThread.Join();

        if (connectionFailed)
        {
            IniNavigationStore.Close();
        }
        else
        {
            try
            {
                AttemptResultNavigator.NavigateAndClose(this, _navigationStore, CurrentAttempt.Id);
            }
            catch
            {
                await ShowPopup(header: Localization.ErrorCaption, message: Localization.SaveError);
            }
        }
    }

    public void SwitchToPathInTarget(Point2D<int> userShot)
    {
        if (PathToTargetStopwatch is null)
        {
            return;
        }
        PathToTargetStopwatch.Stop();

        if (userShot.X != 0 && userShot.Y != 0)
        {
            PathsToTargets[TargetId - 1].Add(Converter.ToAngle_FromWnd(userShot));
        }
        PathsInTargets.Add([]);

        double distance = 0;
        var pathToTarget = PathsToTargets[TargetId - 1];
        for (int i = 1; i < pathToTarget.Count; i++)
        {
            distance += pathToTarget[i - 1].GetDistance(pathToTarget[i]);
        }

        var touchPoint = Converter.ToAngle_FromWnd(userShot);
        var time = PathToTargetStopwatch.Elapsed.TotalSeconds;
        var avgSpeed = distance / time;
        var approachSpeed = pathToTarget[0].GetDistance(touchPoint) / time;

        var ptt = new PathToTarget()
        {
            Distance = distance,
            AverageSpeed = avgSpeed,
            ApproachSpeed = approachSpeed,
            CoordinatesJson = JsonConvert.SerializeObject(PathsToTargets[TargetId - 1]),
            TargetNum = TargetId - 1,
            Attempt = CurrentAttempt.Id,
            Time = time
        };

        _ = Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            _ = await _database.PathToTargets.AddAsync(ptt);
            _ = await _database.SaveChangesAsync();
        });
    }

    public bool SwitchToPathToTarget(IProgressTarget target)
    {
        if (PathToTargetStopwatch is null)
        {
            return false;
        }

        var pathInTarget = PathsInTargets[TargetId - 1];
        PathsToTargets.Add([]);

        var accuracy = (float)pathInTarget.Count(p => target.Contains(Converter.ToWndCoord(p))) / pathInTarget.Count;

        var pit = new PathInTarget()
        {
            CoordinatesJson = JsonConvert.SerializeObject(pathInTarget),
            Attempt = CurrentAttempt.Id,
            TargetId = TargetId - 1,
            Accuracy = accuracy
        };

        _ = Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            _ = await _database.PathInTargets.AddAsync(pit);
            _ = await _database.SaveChangesAsync();
        });

        var newCenter = NextTargetCenter;
        target.Reset();

        if (newCenter is not null)
        {
            var wndCenter = Converter.ToWndCoord(newCenter);
            target.Move(wndCenter);

            PathToTargetStopwatch.Restart();

            return true;
        }
        return false;
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);

        IsGame = false;
        if (DiskNetworkThread.IsAlive)
        {
            DiskNetworkThread.Join();
        }
    }
}
