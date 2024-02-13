using Disk.Calculations.Impl;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FilePath = System.IO.Path;
using Path = Disk.Visual.Impl.Path;
using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using Point3DF = Disk.Data.Impl.Point3D<float>;
using PolarPointF = Disk.Data.Impl.PolarPoint<float>;
using Settings = Disk.Config.Config;
using Timer = System.Timers.Timer;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for PaintWindow.xaml
    /// </summary>
    public partial class PaintWindow : Window
    {
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

        private readonly Random Random = new();

        private readonly Timer ShotTimer;
        private readonly Timer MoveTimer;

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

        private Axis? XAxis;
        private Axis? YAxis;
        private Axis? PaintToDataBorder;

        private User? User;

        private Target? Target;

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

        private string MovingToTargetLogName => $"{CurrPath}{FilePath.DirectorySeparatorChar}Движение к мишени {TargetID}.log";
        private string OnTargetLogName => $"{CurrPath}{FilePath.DirectorySeparatorChar}В мишени {TargetID - 1}.log";
        private string TargetReachedLogName => $"{CurrPath}{FilePath.DirectorySeparatorChar}Мишень {TargetID - 1} поражена.log";

        private string UsrWndLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_WND_LOG_FILE}";
        private string UsrAngLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_ANG_LOG_FILE}";
        private string UsrCenLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}{Settings.USER_CEN_LOG_FILE}";
        private string UsrMovementLog => $"{CurrPath}{FilePath.DirectorySeparatorChar}До первой цели.log";

        private int Score = 0;
        private int TargetID = 1;

        private bool IsGame = true;

        public PaintWindow()
        {
            InitializeComponent();

            NetworkThread = new(NetworkReceive);

            MoveTimer = new(Settings.MOVE_TIME);
            MoveTimer.Elapsed += MoveTimerElapsed;

            ShotTimer = new(Settings.SHOT_TIME);
            ShotTimer.Elapsed += ShotTimerElapsed;

            Closing += OnClosing;
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            MouseLeftButtonDown += OnMouseLeftButtonDown;

            CbTargets.SelectionChanged += CbTargets_SelectionChanged;
            RbPath.Checked += RbPath_Checked;
            RbRose.Checked += RbRose_Checked;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsGame)
            {
                var mousePos = e.GetPosition(sender as UIElement);

                Target?.Move(new((int)mousePos.X, (int)mousePos.Y));
                TargetCenters.Add(new(Converter?.ToAngleX_FromWnd((int)mousePos.X) ?? 0.0f,  
                    Converter?.ToAngleY_FromWnd((int)mousePos.Y) ?? 0.0f));

                Stopwatch = Stopwatch.StartNew();
                TblTime.Text = string.Empty;

                if (User is not null)
                {
                    StartPoint = Converter?.ToAngle_FromWnd(User.Center);
                }

                lock (LockObject)
                {
                    UserMovementLog?.Dispose();
                    UserMovementLog = Logger.GetLogger(MovingToTargetLogName);
                }

                TargetID++;
            }
        }

        private void ShotTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (Target is not null && User is not null)
            {
                var shotScore = Target.ReceiveShot(User.Shot());

                if (shotScore != 0 && Stopwatch.IsRunning)
                {
                    Stopwatch.Stop();

                    if (StartPoint is not null)
                    {
                        using var log = Logger.GetLogger(TargetReachedLogName);

                        var touchPoint = Converter?.ToAngle_FromWnd(User.Center);
                        var distance = touchPoint?.GetDistance(StartPoint);
                        var time = Stopwatch.Elapsed.TotalSeconds;
                        var avgSpeed = distance / time;

                        var message =
                            $"""
                            Время: {time:F2}
                            Расстояние(в углах): {distance:F2}
                            Средняя скорость(в углах): {avgSpeed:F2}
                            """;

                        Application.Current.Dispatcher.Invoke(() => TblTime.Text = message);
                        log.Log(message);

                        lock (LockObject)
                        {
                            UserMovementLog?.Dispose();
                            UserMovementLog = Logger.GetLogger(OnTargetLogName);
                        }
                    }
                }

                Score += shotScore;
            }

            Application.Current.Dispatcher.Invoke(() => Title = $"Счет: {Score}");
            Application.Current.Dispatcher.Invoke(() => TblScore.Text = $"Счет: {Score}");
        }

        private void OnClosing(object? sender, CancelEventArgs e) => StopGame();

        private void MoveTimerElapsed(object? sender, ElapsedEventArgs e)
            => Application.Current.Dispatcher.Invoke(() => User?.Move(ShiftedWndPos ?? User.Center));

        private void NetworkReceive()
        {
/*            try
            {
                using var con = Connection.GetConnection(IPAddress.Parse(Settings.IP), Settings.PORT);

                while (IsGame)
                {
                    CurrentPos = con.GetXYZ();
                }
            }
            catch
            {
                MessageBox.Show("Соединение потеряно");
                Application.Current.Dispatcher.BeginInvoke(new Action(() => Close()));
            }*/
        }

        private void ShowStats()
        {
            using var userAngleReader = FileReader<float>.Open(UsrAngLog, Settings.LOG_SEPARATOR);

            var dataset = userAngleReader.Get2DPoints().ToList();

            if (dataset.Count != 0)
            {
                var mx = Calculator2D.MathExp(dataset);
                var dispersion = Calculator2D.Dispersion(dataset);
                var deviation = Calculator2D.StandartDeviation(dataset);

                MessageBox.Show(
                $"""
                Счет: {Score}
                Среднее смещение от центра: {mx}
                Дисперсия: {dispersion}
                Среднее отклонение от центра: {deviation}
                """);
            }
            else
            {
                MessageBox.Show("Конец");
            }
        }

        private void StopGame()
        {
            if (Target is not null)
            {
                Target.Remove(PaintArea.Children);
                User?.Remove(PaintArea.Children);

                Drawables.Remove(Target);
                Scalables.Remove(Target);

                Drawables.Remove(User);
                Scalables.Remove(User);
            }

            IsGame = false;

            NetworkThread.Join();

            MoveTimer.Stop();
            ShotTimer.Stop();

            UserLogAng?.Dispose();
            UserLogWnd?.Dispose();
            UserLogCen?.Dispose();
            UserMovementLog?.Dispose();
        }

        // replace with btn start
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(CurrPath))
            {
                Directory.CreateDirectory(CurrPath);
            }

            UserLogWnd = Logger.GetLogger(UsrWndLog);
            UserLogAng = Logger.GetLogger(UsrAngLog);
            UserLogCen = Logger.GetLogger(UsrCenLog);
            UserMovementLog = Logger.GetLogger(UsrMovementLog);

            Converter = new(SCREEN_INI_SIZE, new(X_ANGLE_SIZE, Y_ANGLE_SIZE));

            XAxis = new(new(0, SCREEN_INI_CENTER_X), new((int)SCREEN_INI_SIZE.Width, SCREEN_INI_CENTER_Y), SCREEN_INI_SIZE,
                Brushes.Black);
            YAxis = new(new(SCREEN_INI_CENTER_X, 0), new(SCREEN_INI_CENTER_X, (int)SCREEN_INI_SIZE.Height), SCREEN_INI_SIZE,
                Brushes.Black);
            PaintToDataBorder = new(new((int)SCREEN_INI_SIZE.Width, 0), new((int)SCREEN_INI_SIZE.Width,
                (int)SCREEN_INI_SIZE.Height), SCREEN_INI_SIZE, Brushes.Black);

            User = new(new(SCREEN_INI_CENTER_X, SCREEN_INI_CENTER_Y), Settings.USER_INI_RADIUS, Settings.USER_INI_SPEED,
                UserBrush, SCREEN_INI_SIZE);
            User.OnShot += UserLogWnd.LogLn;
            User.OnShot += (p) => UserLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            User.OnShot += (p) => UserLogCen.LogLn(Converter?.ToLogCoord(p));
            User.OnShot += (p) => UserMovementLog.LogLn(Converter?.ToAngle_FromWnd(p));

            Target = new(new(-Settings.TARGET_INI_RADIUS * 10, -Settings.TARGET_INI_RADIUS * 10), Settings.TARGET_INI_RADIUS,
                SCREEN_INI_SIZE);

            Drawables.Add(XAxis); Drawables.Add(YAxis); Drawables.Add(PaintToDataBorder); Drawables.Add(Target);
            Drawables.Add(User);
            Scalables.Add(XAxis); Scalables.Add(YAxis); Scalables.Add(PaintToDataBorder); Scalables.Add(Target);
            Scalables.Add(User); Scalables.Add(Converter);

            foreach (var elem in Drawables)
            {
                elem?.Draw(PaintArea);
            }

            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintPanelSize);
            }

            NetworkThread.Start();

            MoveTimer.Start();
            ShotTimer.Start();
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintPanelSize);
            }
        }

        private void CbTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PaintArea.Children.Clear();
            foreach (var elem in Drawables)
            {
                elem?.Draw(PaintArea);
            }

            var selectedIndex = CbTargets.SelectedIndex;
            var roseFileName = $"{CurrPath}{FilePath.DirectorySeparatorChar}В мишени {selectedIndex + 1}.log";
            var pathFileName = $"{CurrPath}{FilePath.DirectorySeparatorChar}Движение к мишени {selectedIndex + 1}.log";

            if (selectedIndex != -1 && Converter is not null && Target is not null)
            {
                if (RbRose.IsChecked ?? false)
                {
                    if (File.Exists(roseFileName))
                    {
                        using var userReader = FileReader<float>.Open(roseFileName, Settings.LOG_SEPARATOR);

                        var angRadius = Converter.ToAngleX_FromLog(Target.Radius) + 
                            Converter.ToAngleY_FromLog(Target.Radius) / 2;

                        var a = userReader.Get2DPoints().ToList();
                        var dataset =
                            a
                            .Select(p => new PolarPointF(p.X - TargetCenters[selectedIndex].X, p.Y - 
                            TargetCenters[selectedIndex].Y, null))
                            .Where(p => Math.Abs(p.X) > angRadius && Math.Abs(p.Y) > angRadius).ToList();

                        var userRose = new Graph(dataset, PaintPanelSize, Brushes.LightGreen, 8);

                        userRose.Draw(PaintArea);

                        Scalables.Add(userRose);
                    }
                }
                else if (RbPath.IsChecked ?? false)
                {
                    if (File.Exists(pathFileName))
                    {
                        using var userPathReader = FileReader<float>.Open(pathFileName, Settings.LOG_SEPARATOR);

                        var userPath = new Path(userPathReader.Get2DPoints(), PaintPanelSize, new(X_ANGLE_SIZE, Y_ANGLE_SIZE),
                            new SolidColorBrush(Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G, 
                            Settings.USER_COLOR.B)));

                        userPath.Draw(PaintArea);

                        Scalables.Add(userPath);
                    }
                }
            }
        }

        private void RbRose_Checked(object sender, RoutedEventArgs e)
        {
            CbTargets.Items.Clear();

            for (int i = 1; i < TargetID; i++)
            {
                CbTargets.Items.Add($"Роза ветров для цели {i}");
            }
        }

        private void RbPath_Checked(object sender, RoutedEventArgs e)
        {
            CbTargets.Items.Clear();

            for (int i = 1; i < TargetID; i++)
            {
                CbTargets.Items.Add($"Путь к цели {i}");
            }
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            StopGame();
            ShowStats();

            BtnStop.IsEnabled = false;

            for (int i = 1; i < TargetID; i++)
            {
                CbTargets.Items.Add($"Роза ветров для цели {i}");
            }
            CbTargets.Visibility = Visibility.Visible;
            RbPath.Visibility = Visibility.Visible;
            RbRose.Visibility = Visibility.Visible;
        }
    }
}
