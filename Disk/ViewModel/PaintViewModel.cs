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
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Threading;
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

    // Scale
    public Converter Converter { get; private set; }

    // Get only
    public Point2D<int>? TargetCenter => TargetCenters.Count <= TargetId ? null : Converter.ToWndCoord(TargetCenters[TargetId]);
    private static Settings Settings => Settings.Default;
    private string UsrAngLog => $"{CurrentAttempt.LogFilePath}{FilePath.DirectorySeparatorChar}{Settings.UserLogFileName}";
    public bool IsPathToTarget => _pathToTargetStopwatch?.IsRunning ?? false;

    // Disposable
    private Thread _diskNetworkThread;

    // Attempts datasets
    public List<Point2D<float>> TargetCenters { get; private set; } = [];

    public readonly List<Point2D<float>> FullPath = [];
    public readonly List<List<Point2D<float>>> PathsToTargets = [[]];
    public readonly List<List<Point2D<float>>> PathsInTargets = [];

    // Changing
    public Point3D<float>? CurrentPos { get; private set; }
    public bool IsGame { get; private set; } = false;
    public int TargetId { get; private set; }
    public int PttLastSavedId { get; private set; } = -1;
    public int PitLastSavedId { get; private set; } = -1;
    public int HitsCount { get; set; }
    public int ShotsCount { get; set; }


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

    // binding
    public string ScoreString => $"{Localization.Score}: {Score}";

    private bool _isStopEnabled = true;
    public bool IsStopEnabled { get => _isStopEnabled; set => SetProperty(ref _isStopEnabled, value); }

    // DI
    private readonly NavigationStore _navigationStore;
    private readonly ModalNavigationStore _modalNavigationStore;
    private readonly DiskContext _database;

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

            IsGame = true;
            IsStopEnabled = true;
            while (IsGame)
            {
                CurrentPos = con.GetXYZ();
            }
        }
        catch (Exception ex)
        {
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
            _ = await _database.SaveChangesAsync();

            using (var logger = Logger.GetLogger(UsrAngLog))
            {
                FullPath.ForEach(logger.LogLn);
            }

            FullPath.Clear();
        }
        else
        {
            _database.Attempts.Remove(CurrentAttempt);
            _database.SaveChanges();
        }
    }

    public void StopRecord()
    {
        IsStopEnabled = false;
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
            TargetNum = TargetId,
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
            var wndCenter = Converter.ToWndCoord(TargetCenter);
            target.Move(wndCenter);

            _pathToTargetStopwatch.Restart();

            return true;
        }
        return false;
    }

    public void SavePathInTarget()
    {
        if (PathsInTargets[TargetId].Count == 0) return;

        var pathInTarget = PathsInTargets[TargetId];
        float accuracy = 0;

        if (ShotsCount > 0)
        {
            accuracy = (float)HitsCount / ShotsCount;
        }

        var pit = new PathInTarget()
        {
            CoordinatesJson = JsonConvert.SerializeObject(pathInTarget),
            Attempt = CurrentAttempt.Id,
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
