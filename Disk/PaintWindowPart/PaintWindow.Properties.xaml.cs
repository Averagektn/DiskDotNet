using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.ViewModel;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FilePath = System.IO.Path;
using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using Point3DF = Disk.Data.Impl.Point3D<float>;
using Settings = Disk.Properties.Config.Config;

namespace Disk
{
    public partial class PaintWindow : Window
    {
        public const int TargetHP = 20;

        public string MapFilePath = string.Empty;
        public string CurrPath = string.Empty;

        private static readonly object LockObject = new();

        private static readonly Brush UserBrush =
            new SolidColorBrush(Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G, Settings.USER_COLOR.B));

        private static readonly Size SCREEN_INI_SIZE = new(Settings.SCREEN_INI_WIDTH, Settings.SCREEN_INI_HEIGHT);
        private static readonly int SCREEN_INI_CENTER_X = (int)SCREEN_INI_SIZE.Width / 2;
        private static readonly int SCREEN_INI_CENTER_Y = (int)SCREEN_INI_SIZE.Height / 2;

        private static readonly float X_ANGLE_SIZE = Settings.X_MAX_ANGLE * 2;
        private static readonly float Y_ANGLE_SIZE = Settings.Y_MAX_ANGLE * 2;

        private static Settings Settings => Settings.Default;

        private readonly DispatcherTimer ShotTimer;
        private readonly DispatcherTimer MoveTimer;

        private readonly Thread NetworkThread;

        private readonly List<IScalable?> Scalables = [];
        private readonly List<IDrawable?> Drawables = [];

        private readonly List<Point2DF> TargetCenters = [];

        private Stopwatch Stopwatch = new();

        private Point2DF? StartPoint;

        private Logger? UserLogWnd;
        private Logger? UserLogCen;
        private Logger? UserLogAng;
        private Logger? UserMovementLog;

        private FileReader<float>? MapReader;

        private UserPicture? User;
        //private User? User;

        private ProgressTarget? Target;

        private Converter? Converter;

        private Point3DF? CurrentPos;

        private Point2DI? ShiftedWndPos
        {
            get
            {
                if (User is null)
                {
                    return null;
                }

                if (Converter is null || CurrentPos is null)
                {
                    return User.Center;
                }

                return Converter.ToWndCoord(
                    new Point2DF(CurrentPos.X - Settings.ANGLE_X_SHIFT, CurrentPos.Y - Settings.ANGLE_Y_SHIFT));
            }
        }

        private Size ScreenSize => PaintAreaGrid.RenderSize;
        private int ScreenCenterX => (int)ScreenSize.Width / 2;
        private int ScreenCenterY => (int)ScreenSize.Height / 2;

        private Size PaintPanelSize => PaintRect.RenderSize;
        private int PaintPanelCenterX => (int)PaintPanelSize.Width / 2;
        private int PaintPanelCenterY => (int)PaintPanelSize.Height / 2;

        private Size DataPanelSize => DataRect.RenderSize;

        private string MovingToTargetLogName => GetMovToTargetFileName(TargetID);
        private string OnTargetLogName => GetInTargetFileName(TargetID);
        private string TargetReachedLogName => GetReachedFileName(TargetID);

        private string UsrWndLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_WND_LOG_FILE}";
        private string UsrAngLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_ANG_LOG_FILE}";
        private string UsrCenLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_CEN_LOG_FILE}";

        private int Score = 0;
        private int TargetID = 1;

        private bool IsGame = true;

        //private PaintViewModel PaintViewModel => (DataContext as PaintViewModel)!;

        public PaintWindow()
        {
            InitializeComponent();

            NetworkThread = new(NetworkReceive);

            Stopwatch = Stopwatch.StartNew();

            MoveTimer = new(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.MOVE_TIME)
            };
            MoveTimer.Tick += MoveTimerElapsed;

            ShotTimer = new(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.SHOT_TIME)
            };
            ShotTimer.Tick += ShotTimerElapsed;

            Closing += OnClosing;
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;

            CbTargets.SelectionChanged += CbTargets_SelectionChanged;
            RbPath.Checked += RbPath_Checked;
            RbRose.Checked += RbRose_Checked;

            User = new("/Properties/pngegg.png", new(SCREEN_INI_CENTER_X, SCREEN_INI_CENTER_Y), Settings.USER_INI_SPEED,
                new(50, 50), SCREEN_INI_SIZE);
            //User = new(new(SCREEN_INI_CENTER_X, SCREEN_INI_CENTER_Y), 5, 5, Brushes.Green, SCREEN_INI_SIZE);
        }
    }
}
