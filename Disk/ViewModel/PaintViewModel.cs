using Disk.Calculations.Impl;
using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Service.Implementation;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using FilePath = System.IO.Path;
using Localization = Disk.Properties.Localization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class PaintViewModel : ObserverViewModel
    {
        // Can set on creation
        public event Action? OnSessionOver;
        public string ImagePath = "/Properties/pngegg.png";
        public string CurrPath { get; set; } = null!;

        // Scale
        public Converter Converter { get; set; }

        // Properties
        public Point2D<float>? NextTargetCenter => TargetCenters.Count <= TargetId ? null : TargetCenters[TargetId++];
        private static Settings Settings => Settings.Default;
        private string UsrAngLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_ANG_LOG_FILE}";
        public bool IsPathToTarget => PathToTargetStopwatch.IsRunning;

        // Disposable
        private readonly Thread DiskNetworkThread;
        private Logger UserMovementLog = null!;

        // sessions datasets
        public List<Point2D<float>> TargetCenters { get; set; } = null!;
        public List<Point2D<float>> FullPath = [];
        public List<List<Point2D<float>>> PathsToTargets = [[]];
        public List<List<Point2D<float>>> PathsInTargets = [];

        // changing
        public Point3D<float>? CurrentPos;
        public bool IsGame = true;
        public int TargetId { get; set; }
        public int Score { get; set; }
        public ObservableCollection<string> PathsAndRoses { get; set; } = [];

        // DI
        private readonly NavigationStore _navigationStore;
        private readonly IPathToTargetRepository _pathToTargetRepository;
        private readonly IPathInTargetRepository _pathInTargetRepository;
        private readonly ISessionResultRepository _sessionResultRepository;

        // readonly
        private readonly Stopwatch PathToTargetStopwatch;

        // binding
        public string ScoreString => $"{Localization.Paint_Score}: {Score}";
        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }
        private bool _isStopEnabled = true;
        public bool IsStopEnabled { get => _isStopEnabled; set => SetProperty(ref _isStopEnabled, value); }
        private Visibility _rosesAndPathsVisibility = Visibility.Hidden;
        public Visibility RosesAndPathsVisibility { get => _rosesAndPathsVisibility; set => SetProperty(ref _rosesAndPathsVisibility, value); }
        private Visibility _pathButtonVisibility = Visibility.Hidden;
        public Visibility PathButtonVisibility { get => _pathButtonVisibility; set => SetProperty(ref _pathButtonVisibility, value); }
        private Visibility _roseButtonVisibility = Visibility.Hidden;
        public Visibility RoseButtonVisibility { get => _roseButtonVisibility; set => SetProperty(ref _roseButtonVisibility, value); }

        // commands
        public ICommand RoseSelectedCommand => new Command(RoseSelected);
        public ICommand PathSelectedCommand => new Command(PathSelected);

        private void PathSelected(object? obj)
        {
            PathsAndRoses.Clear();

            for (int i = 1; i < TargetId + 1; i++)
            {
                PathsAndRoses.Add($"{Localization.Paint_PathToTarget} {i}");
            }
        }

        private void RoseSelected(object? obj)
        {
            PathsAndRoses.Clear();

            for (int i = 1; i < TargetId + 1; i++)
            {
                PathsAndRoses.Add($"{Localization.Paint_WindRoseForTarget} {i}");
            }
        }

        public PaintViewModel(NavigationStore navigationStore, IPathToTargetRepository pathToTargetRepository,
            IPathInTargetRepository pathInTargetRepository, ISessionResultRepository sessionResultRepository)
        {
            DiskNetworkThread = new(ReceiveUserPos);
            DiskNetworkThread.Start();

            Converter = DrawableFabric.GetConverter();

            _navigationStore = navigationStore;
            _pathToTargetRepository = pathToTargetRepository;
            _pathInTargetRepository = pathInTargetRepository;
            _sessionResultRepository = sessionResultRepository;

            PathToTargetStopwatch = Stopwatch.StartNew();
        }

        public ProgressTarget GetProgressTarget()
        {
            var center = NextTargetCenter ?? new(0.5f, 0.5f);
            var wndCenter = Converter.ToWnd_FromRelative(center);

            var target = DrawableFabric.GetProgressTarget(wndCenter);
            target.OnReceiveShot += shot => Score += shot;

            return target;
        }

        public User GetUser()
        {
            var user = DrawableFabric.GetUser(ImagePath);

            UserMovementLog = Logger.GetLogger(UsrAngLog);
            user.OnShot += (p) => UserMovementLog.LogLn(Converter.ToAngle_FromWnd(p));
            user.OnShot += (p) => FullPath.Add(Converter.ToAngle_FromWnd(p));

            return user;
        }

        private void ReceiveUserPos()
        {
            try
            {
                using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.PORT);

                while (IsGame)
                {
                    CurrentPos = con.GetXYZ();
                }
            }
            catch
            {
                _ = MessageBox.Show(Localization.Paint_ConnectionLost);
                _ = Application.Current.Dispatcher.BeginInvoke(new Action(() => _navigationStore.NavigateBack()));
            }
        }

        public void SaveSessionResult()
        {
            var mx = Calculator2D.MathExp(FullPath);
            var dispersion = Calculator2D.Dispersion(FullPath);
            var deviation = Calculator2D.StandartDeviation(FullPath);

            var sres = new SessionResult()
            {
                Id = AppointmentSession.CurrentSession.Id,
                MathExp = (mx.XDbl + mx.YDbl) / 2,
                Dispersion = (dispersion.XDbl + dispersion.YDbl) / 2,
                Deviation = (deviation.XDbl + dispersion.YDbl) / 2,
                Score = Score
            };

            _sessionResultRepository.Add(sres);

            IsStopEnabled = false;

            RoseSelected(null);

            RosesAndPathsVisibility = Visibility.Visible;
            RoseButtonVisibility = Visibility.Visible;
            PathButtonVisibility = Visibility.Visible;

            OnSessionOver?.Invoke();
        }

        public void SavePathToTarget(PathToTarget pathToTarget) => _pathToTargetRepository.Add(pathToTarget);
        public void SavePathInTarget(PathInTarget pathInTarget) => _pathInTargetRepository.Add(pathInTarget);

        public override void Dispose()
        {
            GC.SuppressFinalize(this);

            IsGame = false;
            UserMovementLog.Dispose();
            DiskNetworkThread.Join();
        }

        public void SwitchToPathInTarget(Point2D<int> userShot)
        {
            PathToTargetStopwatch.Stop();

            PathsToTargets[TargetId - 1].Add(Converter.ToAngle_FromWnd(userShot));
            PathToTargetStopwatch.Stop();
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
                AngleDistance = distance,
                AngleSpeed = avgSpeed,
                ApproachSpeed = approachSpeed,
                CoordinatesJson = JsonConvert.SerializeObject(PathsToTargets[TargetId - 1]),
                TargetNum = TargetId - 1,
                Session = AppointmentSession.CurrentSession.Id,
                Time = time
            };
            SavePathToTarget(ptt);

            Message =
                    $"""
                        {Localization.Paint_Time}: {time:F2}
                        {Localization.Paint_AngleDistance}: {distance:F2}
                        {Localization.Paint_AngleSpeed}: {avgSpeed:F2}
                        {Localization.Paint_ApproachSpeed}: {approachSpeed:F2}
                     """;
        }

        public bool SwitchToPathToTarget(ProgressTarget target)
        {
            PathsToTargets.Add([]);

            var pit = new PathInTarget()
            {
                CoordinatesJson = JsonConvert.SerializeObject(PathsInTargets[TargetId - 1]),
                Session = AppointmentSession.CurrentSession.Id,
                TargetId = TargetId,
            };

            SavePathInTarget(pit);

            // TargetId++
            var newCenter = NextTargetCenter;

            target.Reset();

            if (newCenter is not null)
            {
                var wndCenter = Converter.ToWnd_FromRelative(newCenter);
                target.Move(wndCenter);

                Message = string.Empty;
                PathToTargetStopwatch.Restart();

                return true;
            }
            return false;
        }
    }
}