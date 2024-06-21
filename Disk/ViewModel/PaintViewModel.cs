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
using Disk.Visual.Interface;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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

        // Scale
        public Converter Converter { get; set; }

        // Properties
        public Point2D<float>? NextTargetCenter => TargetCenters.Count <= TargetId ? null : TargetCenters[TargetId++];
        private static Settings Settings => Settings.Default;
        private string UsrAngLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.UserLogFileName}";
        public bool IsPathToTarget => PathToTargetStopwatch.IsRunning;

        // Disposable
        private readonly Thread DiskNetworkThread;

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
        private readonly ISessionRepository _sessionRepository;

        // readonly
        private readonly Stopwatch PathToTargetStopwatch;

        // binding
        public bool IsRoseChecked { get; set; }
        public bool IsPathChecked { get; set; }
        private int _selectedRoseOrPath = -1;
        public int SelectedRoseOrPath { get => _selectedRoseOrPath; set => SetProperty(ref _selectedRoseOrPath, value); }
        public string ScoreString => $"{Localization.Score}: {Score}";

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
        public ICommand SelectionChangedCommand => new Command(SelectionChanged);

        public PaintViewModel(NavigationStore navigationStore, IPathToTargetRepository pathToTargetRepository,
            IPathInTargetRepository pathInTargetRepository, ISessionResultRepository sessionResultRepository, ISessionRepository sessionRepository)
        {
            DiskNetworkThread = new(ReceiveUserPos);
            DiskNetworkThread.Start();

            Converter = DrawableFabric.GetIniConverter();

            _navigationStore = navigationStore;
            _pathToTargetRepository = pathToTargetRepository;
            _pathInTargetRepository = pathInTargetRepository;
            _sessionResultRepository = sessionResultRepository;
            _sessionRepository = sessionRepository;

            PathToTargetStopwatch = Stopwatch.StartNew();
        }

        public List<IStaticFigure> GetPathAndRose(Target target, Size paintAreaSize)
        {
            if (SelectedRoseOrPath == -1)
            {
                return [];
            }

            if (IsRoseChecked)
            {
                return [GetGraph(target, paintAreaSize)];
            }
            else if (IsPathChecked)
            {
                if (PathsToTargets.Count <= SelectedRoseOrPath || PathsToTargets[SelectedRoseOrPath].Count == 0)
                {
                    MessageBox.Show(Localization.NoContentForPathError);
                    return [];
                }
                var converter = DrawableFabric.GetIniConverter();
                var path = new Path
                    (
                        PathsToTargets[SelectedRoseOrPath], Converter,
                        new SolidColorBrush(Color.FromRgb(Settings.UserColor.R, Settings.UserColor.G, Settings.UserColor.B))
                    );
                var targetCenter = converter.ToWnd_FromRelative(TargetCenters[SelectedRoseOrPath]);
                var targetToDraw = DrawableFabric.GetIniProgressTarget(targetCenter);

                var userToDraw = DrawableFabric.GetIniUser(ImagePath);
                userToDraw.Move(converter.ToWndCoord(PathsToTargets[SelectedRoseOrPath][0]));

                return [targetToDraw, userToDraw, path];
            }
            return [];
        }

        private Graph GetGraph(Target target, Size paintAreaSize)
        {
            if (PathsInTargets.Count <= SelectedRoseOrPath || PathsInTargets[SelectedRoseOrPath].Count == 0)
            {
                MessageBox.Show(Localization.NoContentForDiagramError);
                return new Graph([], paintAreaSize, Brushes.LightGreen, 8);
            }

            var angRadius = (Converter.ToAngleX_FromWnd(target.Radius) + Converter.ToAngleY_FromWnd(target.Radius)) / 2;

            var angCenter = Converter.ToAngle_FromWnd(Converter.ToWnd_FromRelative(TargetCenters[SelectedRoseOrPath]));
            var dataset =
                PathsInTargets[SelectedRoseOrPath]
                .Select(p => new PolarPoint<float>(p.X - angCenter.X, p.Y - angCenter.Y))
                .Where(p => Math.Abs(p.X) > angRadius && Math.Abs(p.Y) > angRadius)
                .ToList();

            return new Graph(dataset, paintAreaSize, Brushes.LightGreen, 8);
        }

        private void SelectionChanged(object? obj)
        {
            SelectedRoseOrPath = -1;
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
                    Id = AppointmentSession.CurrentSession.Id,
                    MathExp = (mx.XDbl + mx.YDbl) / 2,
                    Dispersion = (dispersion.XDbl + dispersion.YDbl) / 2,
                    Deviation = (deviation.XDbl + dispersion.YDbl) / 2,
                    Score = Score
                };

                _sessionResultRepository.Add(sres);
            }
            else
            {
                _sessionRepository.Delete(AppointmentSession.CurrentSession);
            }

            using (var logger = Logger.GetLogger(UsrAngLog))
            {
                foreach (var point in FullPath)
                {
                    logger.LogLn(point);
                }
            }

            IsStopEnabled = false;

            for (int i = 0; i < TargetCenters.Count; i++)
            {
                PathsAndRoses.Add($"{Localization.Target} {i + 1}");
            }

            RosesAndPathsVisibility = Visibility.Visible;
            RoseButtonVisibility = Visibility.Visible;
            PathButtonVisibility = Visibility.Visible;

            IsGame = false;
            DiskNetworkThread.Join();

            OnSessionOver?.Invoke();
        }

        public void SavePathToTarget(PathToTarget pathToTarget) => _pathToTargetRepository.Add(pathToTarget);
        public void SavePathInTarget(PathInTarget pathInTarget) => _pathInTargetRepository.Add(pathInTarget);

        public void SwitchToPathInTarget(Point2D<int> userShot)
        {
            PathToTargetStopwatch.Stop();

            if (userShot.X != 0 && userShot.Y != 0)
            {
                PathsToTargets[TargetId - 1].Add(Converter.ToAngle_FromWnd(userShot));
            }
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
                        {Localization.Time}: {time:F2}
                        {Localization.AngleDistance}: {distance:F2}
                        {Localization.AngleSpeed}: {avgSpeed:F2}
                        {Localization.ApproachSpeed}: {approachSpeed:F2}
                     """;
        }

        public bool SwitchToPathToTarget(ProgressTarget target)
        {
            var pathInTarget = PathsInTargets[TargetId - 1];
            PathsToTargets.Add([]);

            var precision = (float)pathInTarget.Count(p => target.Contains(Converter.ToWndCoord(p))) / pathInTarget.Count;

            var pit = new PathInTarget()
            {
                CoordinatesJson = JsonConvert.SerializeObject(pathInTarget),
                Session = AppointmentSession.CurrentSession.Id,
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

                Message = string.Empty;
                PathToTargetStopwatch.Restart();

                return true;
            }
            return false;
        }
    }
}