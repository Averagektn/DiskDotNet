using Disk.Calculations.Impl.Converters;
using Disk.ViewModel;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using Point3DF = Disk.Data.Impl.Point3D<float>;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View.PaintWindow
{
    public partial class PaintView : UserControl
    {
        private User User = null!;
        private ProgressTarget Target = null!;
        private Converter Converter => ViewModel.Converter;

        private PaintViewModel ViewModel => (PaintViewModel)DataContext;

        // move all to vm
        // replace
        public List<Point2DF> PathToTargetCoords = [];
        public List<Point2DF> PathInTargetCoords = [];
        // with
        public List<List<Point2DF>> PathsToTargets = [];
        public List<List<Point2DF>> PathsInTargets = [];

        private readonly List<Point2DF> TargetCenters = [];

        //public SessionResult SessionResult { get; set; } = new();

        // Non-verified
        private static Settings Settings => Settings.Default;

        private readonly DispatcherTimer ShotTimer;
        private readonly DispatcherTimer MoveTimer;

        private readonly List<IScalable?> Scalables = [];
        private readonly List<IDrawable?> Drawables = [];

        private Stopwatch Stopwatch = new();
        private Point2DF? PathStartingPoint;
        private Point2DI? ShiftedWndPos
        {
            get
            {
                return ViewModel.CurrentPos is null
                    ? User.Center
                    : Converter.ToWndCoord(
                    new Point2DF(ViewModel.CurrentPos.X - Settings.ANGLE_X_SHIFT, ViewModel.CurrentPos.Y - Settings.ANGLE_Y_SHIFT));
            }
        }

        private Size PaintPanelSize => PaintRect.RenderSize;
        private int PaintPanelCenterX => (int)PaintPanelSize.Width / 2;
        private int PaintPanelCenterY => (int)PaintPanelSize.Height / 2;
        private int Score = 0;
        private int TargetID = 1;


        public PaintView()
        {
            InitializeComponent();

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

            Unloaded += OnClosing;
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;

            CbTargets.SelectionChanged += CbTargets_SelectionChanged;
            RbPath.Checked += RbPath_Checked;
            RbRose.Checked += RbRose_Checked;
        }
    }
}
