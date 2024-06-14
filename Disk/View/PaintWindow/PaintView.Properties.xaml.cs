﻿using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Entities;
using Disk.ViewModel;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using FilePath = System.IO.Path;
using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using Point3DF = Disk.Data.Impl.Point3D<float>;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View.PaintWindow
{
    public partial class PaintView : UserControl
    {
        private PaintViewModel ViewModel => (PaintViewModel)DataContext;

        // replace
        public List<Point2DF> PathToTargetCoords = [];
        public List<Point2DF> PathInTargetCoords = [];
        // with
        public List<List<Point2DF>> PathsToTargets = [];
        public List<List<Point2DF>> PathsInTargets = [];

        private readonly List<Point2DF> TargetCenters = [];

        public SessionResult SessionResult { get; set; } = new();

        public const int TargetHP = 20; // to settings

        private static readonly Brush UserBrush =
            new SolidColorBrush(Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G, Settings.USER_COLOR.B));

        private static readonly Size ScreenIniSize = new(Settings.SCREEN_INI_WIDTH, Settings.SCREEN_INI_HEIGHT);
        private static readonly int ScreenIniCenterX = (int)ScreenIniSize.Width / 2;
        private static readonly int ScreenIniCenterY = (int)ScreenIniSize.Height / 2;

        private static readonly float XAngleSize = Settings.X_MAX_ANGLE * 2;
        private static readonly float YAngleSize = Settings.Y_MAX_ANGLE * 2;

        private static Settings Settings => Settings.Default;

        private readonly DispatcherTimer ShotTimer;
        private readonly DispatcherTimer MoveTimer;

        private readonly Thread DiskNetworkThread;

        private readonly List<IScalable?> Scalables = [];
        private readonly List<IDrawable?> Drawables = [];

        private Stopwatch Stopwatch = new();

        private Point2DF? PathStartingPoint;

        private Logger UserMovementLog = null!;

        private User User = null!;

        private ProgressTarget Target = null!;

        private Converter Converter = null!;

        private Point3DF? CurrentPos;

        private Point2DI? ShiftedWndPos
        {
            get
            {
                return CurrentPos is null
                    ? User.Center
                    : Converter.ToWndCoord(
                    new Point2DF(CurrentPos.X - Settings.ANGLE_X_SHIFT, CurrentPos.Y - Settings.ANGLE_Y_SHIFT));
            }
        }

        private Size ScreenSize => PaintAreaGrid.RenderSize;

        private Size PaintPanelSize => PaintRect.RenderSize;
        private int PaintPanelCenterX => (int)PaintPanelSize.Width / 2;
        private int PaintPanelCenterY => (int)PaintPanelSize.Height / 2;

        private string UsrAngLog => $"{ViewModel.CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_ANG_LOG_FILE}";

        private int Score = 0;
        private int TargetID = 1;

        private bool IsGame = true;

        public PaintView()
        {
            InitializeComponent();

            DiskNetworkThread = new(NetworkReceive);

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
