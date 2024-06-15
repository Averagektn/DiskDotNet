using Disk.Calculations.Impl;
using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Service.Implementation;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using System.Net;
using System.Windows;
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

        // Disposable
        private readonly Thread DiskNetworkThread;
        private Logger UserMovementLog = null!;

        // sessions datasets
        public List<Point2D<float>> TargetCenters { get; set; } = null!;
        public List<Point2D<float>> FullPath = [];
        public List<List<Point2D<float>>> PathsToTargets = [];
        public List<List<Point2D<float>>> PathsInTargets = [];

        // changing
        public Point3D<float>? CurrentPos;
        public bool IsGame = true;
        public int TargetId { get; set; }
        public int Score { get; set; }

        // DI
        private readonly NavigationStore _navigationStore;
        private readonly IPathToTargetRepository _pathToTargetRepository;
        private readonly IPathInTargetRepository _pathInTargetRepository;
        private readonly ISessionResultRepository _sessionResultRepository;

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
            OnSessionOver?.Invoke();
        }

        public void SavePathToTarget(PathToTarget pathToTarget) => _pathToTargetRepository.Add(pathToTarget);
        public void SavePathInTarget(PathInTarget pathInTarget) => _pathInTargetRepository.Add(pathInTarget);

        public override void Dispose()
        {
            base.Dispose();

            IsGame = false;
            UserMovementLog.Dispose();
            DiskNetworkThread.Join();
        }
    }
}