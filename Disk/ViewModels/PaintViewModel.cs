using Disk.Calculations.Implementations;
using Disk.Calculations.Implementations.Converters;
using Disk.Data.Impl;
using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Services.Implementations;
using Disk.Stores;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;
using Disk.Visual.Implementations;
using Disk.Visual.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using FilePath = System.IO.Path;
using Localization = Disk.Properties.Langs.PaintWindow.PaintWindowLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModels;

public class PaintViewModel : PopupViewModel
{
    private long _attemptId;
    public required long AttemptId
    {
        get => _attemptId;
        set
        {
            _attemptId = value;
            CurrentAttempt = _database.Attempts
                .Where(s => s.Id == value)
                .Include(s => s.SessionNavigation)
                .Include(s => s.SessionNavigation.MapNavigation)
                .First();
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

    #region Scale
    public Converter Converter { get; private set; }
    #endregion

    #region Get only
    public Point2D<int>? TargetCenter => TargetCenters.Count <= TargetId ? null : Converter.ToWndCoord(TargetCenters[TargetId]);
    private static Settings Settings => Settings.Default;
    private string UsrAngLog => $"{CurrentAttempt.LogFilePath}{FilePath.DirectorySeparatorChar}{Settings.UserLogFileName}";
    public bool IsPathToTarget => _pathToTargetStopwatch?.IsRunning ?? false;
    #endregion

    #region Disposable
    private Thread _diskNetworkThread;
    #endregion

    #region Attempts datasets
    public List<Point2D<float>> TargetCenters { get; private set; } = [];

    public readonly List<Point2D<float>> FullPath = [];
    public readonly List<List<Point2D<float>>> PathsToTargets = [[]];
    public readonly List<List<Point2D<float>>> PathsInTargets = [];
    #endregion

    #region Changing
    public Point3D<float>? CurrentPos { get; private set; }
    public Visibility AdaptationButtonVisibility => IsGame ? Visibility.Collapsed : Visibility.Visible;
    public bool IsReceivingData { get; private set; } = false;
    public int TargetId { get; private set; }
    public int PttLastSavedId { get; private set; } = -1;
    public int PitLastSavedId { get; private set; } = -1;
    public int HitsCount { get; set; }
    public int ShotsCount { get; set; }

    private bool _isGame = false;
    public bool IsGame
    {
        get => _isGame;
        private set
        {
            _ = SetProperty(ref _isGame, value);
            OnPropertyChanged(nameof(AdaptationButtonVisibility));
        }
    }

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

    private Stopwatch? _pathToTargetStopwatch;
    #endregion

    #region Binding
    public string ScoreString => $"{Localization.Score}: {Score}";
    private bool _isStopEnabled = true;
    public bool IsStopEnabled { get => _isStopEnabled; set => SetProperty(ref _isStopEnabled, value); }
    #endregion

    #region DI
    private readonly NavigationStore _navigationStore;
    private readonly ModalNavigationStore _modalNavigationStore;
    private readonly DiskContext _database;
    #endregion

    #region Commands
    public ICommand AdaptationCommand => new Command(_ => IsGame = true);
    #endregion

    public PaintViewModel(NavigationStore navigationStore, DiskContext database, ModalNavigationStore modalNavigationStore)
    {
        _diskNetworkThread = new(ReceivePatientPosition)
        {
            Priority = ThreadPriority.Highest
        };

        Converter = DrawableFabric.GetIniConverter();

        _modalNavigationStore = modalNavigationStore;
        _navigationStore = navigationStore;
        _database = database;
    }

    #region Data receiveing
    public void StartReceiving()
    {
        _diskNetworkThread.Start();
        _pathToTargetStopwatch = Stopwatch.StartNew();
    }

    private void ReceivePatientPosition()
    {
        try
        {
            using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.Port);

            IsReceivingData = true;
            while (IsReceivingData)
            {
                CurrentPos = con.GetXYZ();
            }
        }
        catch (Exception ex)
        {
            IsReceivingData = false;
            IsGame = false;
            IsStopEnabled = false;
            Log.Error($"{ex.Message} \n\n\n {ex.StackTrace}");

            QuestionNavigator.Navigate(this, _modalNavigationStore,
                message: $"{Settings.IP}: {Localization.ConnectionLost} \n {Localization.TryAgainQuestion}",
                afterConfirm: () =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (_diskNetworkThread.IsAlive)
                        {
                            _diskNetworkThread.Join();
                        }
                        _diskNetworkThread = new(ReceivePatientPosition)
                        {
                            Priority = ThreadPriority.Highest
                        };
                        _diskNetworkThread.Start();
                    });
                },
                afterCancel: () =>
                {
                    _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        StopRecord();
                        await ShowResultAsync();
                    }).Task.ContinueWith(e =>
                    {
                        if (e.Exception is not null)
                        {
                            Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
                        }
                    });
                });
        }
    }
    #endregion

    public async Task SaveAttemptResultAsync()
    {
        if (FullPath.Count > 0)
        {
            var sres = new AttemptResult()
            {
                Id = CurrentAttempt.Id,
                Score = Score,
            };

            _ = await _database.AttemptResults.AddAsync(sres);
            _ = await _database.SaveChangesAsync();

            using (var logger = Logger.GetLogger(UsrAngLog))
            {
                FullPath.ForEach(logger.LogLn);
            }

            FullPath.Clear();
        }
        else
        {
            _ = _database.Attempts.Remove(CurrentAttempt);
            _ = _database.SaveChanges();
        }
    }

    public void StopRecord()
    {
        IsStopEnabled = false;
        IsReceivingData = false;
        IsGame = false;
        if (_diskNetworkThread.IsAlive)
        {
            _diskNetworkThread.Join();
        }
    }

    public async Task ShowResultAsync()
    {
        try
        {
            AttemptResultNavigator.NavigateAndClose(this, _navigationStore, CurrentAttempt.Id);
        }
        catch (Exception ex)
        {
            // this exceptions is OK if path is empty
            await ShowPopup(header: Localization.ErrorCaption, message: Localization.SaveError);
            Log.Error($"{ex.Message} \n\n\n {ex.StackTrace}");
        }
    }

    #region Path to target
    public void SwitchToPathInTarget()
    {
        PathsInTargets.Add([]);

        SavePathToTarget();
    }

    public void SavePathToTarget()
    {
        if (_pathToTargetStopwatch is null || PathsToTargets[TargetId].Count == 0)
        {
            return;
        }

        _pathToTargetStopwatch.Stop();

        double distance = 0;
        var pathToTarget = PathsToTargets[TargetId];
        for (int i = 1; i < pathToTarget.Count; i++)
        {
            distance += pathToTarget[i - 1].GetDistance(pathToTarget[i]);
        }

        if (pathToTarget.Count == 0)
        {
            pathToTarget.Add(CurrentPos?.To2D() ?? new Point2D<float>());
        }
        var touchPoint = pathToTarget[^1];

        var time = _pathToTargetStopwatch.Elapsed.TotalSeconds;
        var avgSpeed = distance / time;
        var approachSpeed = pathToTarget[0].GetDistance(touchPoint) / time;

        var ptt = new PathToTarget()
        {
            Distance = distance,
            AverageSpeed = avgSpeed,
            ApproachSpeed = approachSpeed,
            CoordinatesJson = JsonConvert.SerializeObject(PathsToTargets[TargetId]),
            TargetId = TargetId,
            Attempt = CurrentAttempt.Id,
            Time = time
        };

        _ = Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            _ = await _database.PathToTargets.AddAsync(ptt);
            _ = await _database.SaveChangesAsync();
            PttLastSavedId++;
        }).Task.ContinueWith(e =>
        {
            if (e.Exception is not null)
            {
                Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
            }
        });
        ;
    }
    #endregion

    #region Path in target
    public bool SwitchToPathToTarget(IProgressTarget target)
    {
        if (_pathToTargetStopwatch is null)
        {
            return false;
        }

        SavePathInTarget();

        PathsToTargets.Add([]);

        target.Reset();

        TargetId++;
        if (TargetCenter is not null)
        {
            target.Move(TargetCenter);

            _pathToTargetStopwatch.Restart();

            return true;
        }
        return false;
    }

    public void SavePathInTarget()
    {
        if (PathsInTargets[TargetId].Count == 0)
        {
            return;
        }

        var pathInTarget = PathsInTargets[TargetId];
        float accuracy = 0;

        if (ShotsCount > 0)
        {
            accuracy = (float)HitsCount / ShotsCount;
        }

        const int minEllipsePoints = 10;

        double ellipseArea = 0.0;
        double convexHullArea = 0.0;
        if (pathInTarget.Count > minEllipsePoints)
        {
            ellipseArea = BoundingEllipse.GetArea(pathInTarget);
            convexHullArea = ConvexHull.GetArea(pathInTarget);
        }

        var mx = Calculator2D.MathExp(pathInTarget);
        var deviation = Calculator2D.StandartDeviation(pathInTarget);

        var pit = new PathInTarget()
        {
            CoordinatesJson = JsonConvert.SerializeObject(pathInTarget),
            Attempt = CurrentAttempt.Id,
            MathExpX = mx.X,
            MathExpY = mx.Y,
            DeviationX = deviation.X,
            DeviationY = deviation.Y,
            EllipseArea = ellipseArea,
            ConvexHullArea = convexHullArea,
            TargetId = TargetId,
            Accuracy = accuracy
        };

        _ = Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            _ = await _database.PathInTargets.AddAsync(pit);
            _ = await _database.SaveChangesAsync();
            PitLastSavedId++;
        }).Task.ContinueWith(e =>
        {
            if (e.Exception is not null)
            {
                Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
            }
        });
        ;
    }
    #endregion

    // Save attempt result and last path
    // Last path is saved in case of 'backspace' button os recording stop
    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);

        StopRecord();

        if (PttLastSavedId + 1 != TargetCenters.Count && PttLastSavedId < TargetId && IsPathToTarget)
        {
            SavePathToTarget();
        }
        else if (PitLastSavedId + 1 != TargetCenters.Count && PitLastSavedId < TargetId && !IsPathToTarget)
        {
            SavePathInTarget();
        }

        SaveAttemptResultAsync().Wait();
    }
}
