using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using System.IO;
using System.Windows;
using System.Windows.Media;
using FilePath = System.IO.Path;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class PaintViewModel : ObserverViewModel
    {
        public string ImagePath = "/Properties/pngegg.png";
        public Converter Converter { get; set; }
        public event Action? OnSessionOver;
        public string CurrPath { get; set; } = null!;
        public List<Point2D<float>> TargetCenters { get; set; } = null!;
        public bool UserPictureSelected { get; set; } = true;

        public List<Point2D<float>> FullPath = [];
        public List<List<Point2D<float>>> PathsToTargets = [];
        public List<List<Point2D<float>>> PathsInTargets = [];
        public int TargetId { get; set; }

        private static Settings Settings => Settings.Default;
        private string UsrAngLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_ANG_LOG_FILE}";
        private static readonly Size ScreenIniSize = new(Settings.SCREEN_INI_WIDTH, Settings.SCREEN_INI_HEIGHT);
        private static readonly int ScreenIniCenterX = (int)ScreenIniSize.Width / 2;
        private static readonly int ScreenIniCenterY = (int)ScreenIniSize.Height / 2;
        private Logger UserMovementLog = null!;
        private static readonly Brush UserBrush =
            new SolidColorBrush(Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G, Settings.USER_COLOR.B));
        private static readonly float XAngleSize = Settings.X_MAX_ANGLE * 2;
        private static readonly float YAngleSize = Settings.Y_MAX_ANGLE * 2;

        private readonly NavigationStore _navigationStore;
        private readonly IPathToTargetRepository _pathToTargetRepository;
        private readonly IPathInTargetRepository _pathInTargetRepository;
        private readonly ISessionResultRepository _sessionResultRepository;

        private readonly int TargetHp = 20;

        public PaintViewModel(NavigationStore navigationStore, IPathToTargetRepository pathToTargetRepository,
            IPathInTargetRepository pathInTargetRepository, ISessionResultRepository sessionResultRepository)
        {
            Converter = new(ScreenIniSize, new(XAngleSize, YAngleSize));

            _navigationStore = navigationStore;
            _pathToTargetRepository = pathToTargetRepository;
            _pathInTargetRepository = pathInTargetRepository;
            _sessionResultRepository = sessionResultRepository;
        }

        public ProgressTarget GetProgressTarget()
        {
            var center = NextTargetCenter ?? new(0.5f, 0.5f);

            return new ProgressTarget(Converter.ToWnd_FromRelative(center), Settings.TARGET_INI_RADIUS + 5, ScreenIniSize, TargetHp);
        }

        public User GetUser()
        {
            User user;
            if (UserPictureSelected)
            {
                user = new UserPicture(ImagePath, new(ScreenIniCenterX, ScreenIniCenterY), Settings.USER_INI_SPEED,
                    new(50, 50), ScreenIniSize);
            }
            else
            {
                user = new User(new(ScreenIniCenterX, ScreenIniCenterY), Settings.USER_INI_RADIUS, Settings.USER_INI_SPEED,
                    UserBrush, ScreenIniSize);
            }
            UserMovementLog = Logger.GetLogger(UsrAngLog);
            user.OnShot += (p) => UserMovementLog.LogLn(Converter.ToAngle_FromWnd(p));
            user.OnShot += (p) => FullPath.Add(Converter.ToAngle_FromWnd(p));

            return user;
        }

        public void NavigateToAppoinment()
        {
            _navigationStore.SetViewModel<AppointmentViewModel>(vm => vm.Appointment = AppointmentSession.Appointment);
        }

        public Point2D<float>? NextTargetCenter => TargetCenters.Count <= TargetId ? null : TargetCenters[TargetId++];

        public void SaveSessionResult(SessionResult sessionResult)
        {
            sessionResult.Id = AppointmentSession.CurrentSession.Id;
            _sessionResultRepository.Add(sessionResult);
            OnSessionOver?.Invoke();
        }

        public void SavePathToTarget(PathToTarget pathToTarget)
        {
            pathToTarget.Session = AppointmentSession.CurrentSession.Id;
            _pathToTargetRepository.Add(pathToTarget);
        }

        public void SavePathInTarget(PathInTarget pathInTarget)
        {
            pathInTarget.Session = AppointmentSession.CurrentSession.Id;
            _pathInTargetRepository.Add(pathInTarget);
        }
    }
}