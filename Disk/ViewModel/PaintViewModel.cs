using Disk.Calculations.Impl;
using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Service.Implementation;
using Disk.Stores;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Windows;
using FilePath = System.IO.Path;
using Localization = Disk.Properties.Langs.PaintWindow.PaintWindowLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class PaintViewModel : ObserverViewModel
    {
        // Can set on creation
        public event Action? OnSessionOver;
        public string ImagePath = string.Empty;
        public string CurrPath { get; set; } = null!;
        private Session _currentSession = null!;
        public Session CurrentSession
        {
            get => _currentSession;
            set
            {
                _currentSession = value;
                TargetCenters = JsonConvert.DeserializeObject<List<Point2D<float>>>(_currentSession.MapNavigation.CoordinatesJson) ?? [];
            }
        }

        // Scale
        public Converter Converter { get; set; }

        // Get only
        public Point2D<float>? NextTargetCenter => TargetCenters.Count <= TargetId ? null : TargetCenters[TargetId++];
        private static Settings Settings => Settings.Default;
        private string UsrAngLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.UserLogFileName}";
        public bool IsPathToTarget => PathToTargetStopwatch?.IsRunning ?? false;

        // Disposable
        private readonly Thread DiskNetworkThread;

        // Sessions datasets
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

        // binding
        public string ScoreString => $"{Localization.Score}: {Score}";

        private bool _isStopEnabled = true;
        public bool IsStopEnabled { get => _isStopEnabled; set => SetProperty(ref _isStopEnabled, value); }

        // DI
        private readonly NavigationStore _navigationStore;
        private readonly IPathToTargetRepository _pathToTargetRepository;
        private readonly IPathInTargetRepository _pathInTargetRepository;
        private readonly ISessionResultRepository _sessionResultRepository;
        private readonly ISessionRepository _sessionRepository;

        public PaintViewModel(NavigationStore navigationStore, IPathToTargetRepository pathToTargetRepository,
            IPathInTargetRepository pathInTargetRepository, ISessionResultRepository sessionResultRepository, ISessionRepository sessionRepository)
        {
            DiskNetworkThread = new(ReceiveUserPos);

            Converter = DrawableFabric.GetIniConverter();

            _navigationStore = navigationStore;
            _pathToTargetRepository = pathToTargetRepository;
            _pathInTargetRepository = pathInTargetRepository;
            _sessionResultRepository = sessionResultRepository;
            _sessionRepository = sessionRepository;
        }

        public void StartReceiving()
        {
            DiskNetworkThread.Start();
            PathToTargetStopwatch = Stopwatch.StartNew();
        }

        public ProgressTarget GetProgressTarget()
        {
            var center = NextTargetCenter ?? new(0.5f, 0.5f);
            var wndCenter = Converter.ToWnd_FromRelative(center);

            var target = DrawableFabric.GetIniProgressTarget(wndCenter);
            target.OnReceiveShot += shot => Score += shot;

            return target;
        }

        public User GetUser()
        {
            var user = DrawableFabric.GetIniUser(ImagePath);

            user.OnShot += (p) => FullPath.Add(Converter.ToAngle_FromWnd(p));

            return user;
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
                _ = MessageBox.Show(Localization.ConnectionLost);
                _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SaveSessionResult();
                    _ = _navigationStore.NavigateBack();
                }));
            }
        }

        public void SaveSessionResult()
        {
            if (PathsInTargets.Count != 0)
            {
                var mx = Calculator2D.MathExp(FullPath);
                var dispersion = Calculator2D.Dispersion(FullPath);
                var deviation = Calculator2D.StandartDeviation(FullPath);

                var sres = new SessionResult()
                {
                    Id = CurrentSession.Id,
                    MathExp = (mx.XDbl + mx.YDbl) / 2,
                    Dispersion = (dispersion.XDbl + dispersion.YDbl) / 2,
                    Deviation = (deviation.XDbl + dispersion.YDbl) / 2,
                    Score = Score
                };

                _sessionResultRepository.Add(sres);
            }
            else
            {
                _sessionRepository.Delete(CurrentSession);
            }

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

            OnSessionOver?.Invoke();

            _ = _navigationStore.NavigateBack();
            _navigationStore.SetViewModel<SessionResultViewModel>(vm =>
            {
                vm.CurrentSession = CurrentSession;
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            });
        }

        public void SwitchToPathInTarget(Point2D<int> userShot)
        {
            PathToTargetStopwatch!.Stop();

            if (userShot.X != 0 && userShot.Y != 0)
            {
                PathsToTargets[TargetId - 1].Add(Converter.ToAngle_FromWnd(userShot));
            }
            PathToTargetStopwatch!.Stop();
            PathsInTargets.Add([]);

            double distance = 0;
            var pathToTarget = PathsToTargets[TargetId - 1];
            for (int i = 1; i < pathToTarget.Count; i++)
            {
                distance += pathToTarget[i - 1].GetDistance(pathToTarget[i]);
            }

            var touchPoint = Converter.ToAngle_FromWnd(userShot);
            var time = PathToTargetStopwatch!.Elapsed.TotalSeconds;
            var avgSpeed = distance / time;
            var approachSpeed = pathToTarget[0].GetDistance(touchPoint) / time;

            var ptt = new PathToTarget()
            {
                AngleDistance = distance,
                AngleSpeed = avgSpeed,
                ApproachSpeed = approachSpeed,
                CoordinatesJson = JsonConvert.SerializeObject(PathsToTargets[TargetId - 1]),
                TargetNum = TargetId - 1,
                Session = CurrentSession.Id,
                Time = time
            };
            SavePathToTarget(ptt);
        }

        public bool SwitchToPathToTarget(ProgressTarget target)
        {
            var pathInTarget = PathsInTargets[TargetId - 1];
            PathsToTargets.Add([]);

            var precision = (float)pathInTarget.Count(p => target.Contains(Converter.ToWndCoord(p))) / pathInTarget.Count;

            var pit = new PathInTarget()
            {
                CoordinatesJson = JsonConvert.SerializeObject(pathInTarget),
                Session = CurrentSession.Id,
                TargetId = TargetId - 1,
                Precision = precision
            };

            SavePathInTarget(pit);

            // TargetId++
            var newCenter = NextTargetCenter;

            target.Reset();

            if (newCenter is not null)
            {
                var wndCenter = Converter.ToWnd_FromRelative(newCenter);
                target.Move(wndCenter);

                PathToTargetStopwatch!.Restart();

                return true;
            }
            return false;
        }

        public override void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);

            if (CurrentSession.SessionResult is null)
            {
                _sessionRepository.Delete(CurrentSession);
            }
            IsGame = false;
            if (DiskNetworkThread.IsAlive)
            {
                DiskNetworkThread.Join();
            }
        }

        public void SavePathToTarget(PathToTarget pathToTarget) => _pathToTargetRepository.Add(pathToTarget);
        public void SavePathInTarget(PathInTarget pathInTarget) => _pathInTargetRepository.Add(pathInTarget);
    }
}
