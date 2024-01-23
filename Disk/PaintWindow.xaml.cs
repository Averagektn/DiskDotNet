using Disk.Calculations.Impl;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.ComponentModel;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using Point2DF = Disk.Data.Impl.Point2D<float>;
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
        private static readonly Size SCREEN_INI_SIZE = new(Settings.SCREEN_INI_WIDTH, Settings.SCREEN_INI_WIDTH);

        private static readonly float X_ANGLE_SIZE = Settings.X_MAX_ANGLE * 2;
        private static readonly float Y_ANGLE_SIZE = Settings.Y_MAX_ANGLE * 2;

        private static Settings Settings => Settings.Default;

        private static int SCREEN_INI_CENTER_X => (int)SCREEN_INI_SIZE.Width / 2;
        private static int SCREEN_INI_CENTER_Y => (int)SCREEN_INI_SIZE.Height / 2;

        private readonly Random Random = new();

        private readonly Logger UserLogWnd = Logger.GetLogger(Settings.USER_WND_LOG_FILE);
        private readonly Logger UserLogCen = Logger.GetLogger(Settings.USER_CEN_LOG_FILE);
        private readonly Logger UserLogAng = Logger.GetLogger(Settings.USER_ANG_LOG_FILE);

        // rework after multiple enemies added
        private readonly Logger EnemyLogWnd = Logger.GetLogger(Settings.ENEMY_WND_LOG_NAME + Settings.LOG_EXTENSION);
        private readonly Logger EnemyLogCen = Logger.GetLogger(Settings.ENEMY_CEN_LOG_NAME + Settings.LOG_EXTENSION);
        private readonly Logger EnemyLogAng = Logger.GetLogger(Settings.ENEMY_ANG_LOG_NAME + Settings.LOG_EXTENSION);

        private readonly Timer ShotTimer;
        private readonly Timer MoveTimer;
        private readonly Timer TargetTimer;

        private readonly Thread NetworkThread;

        private readonly List<Enemy?> Enemies = new(Settings.ENEMIES_NUM);

        private readonly List<IScalable?> Scalables = [];
        private readonly List<IDrawable?> Drawables = [];

        private int PaintCenterX => (int)PaintAreaGrid.RenderSize.Width / 2;
        private int PaintCenterY => (int)PaintAreaGrid.RenderSize.Height / 2;

        private Size PaintSize => PaintAreaGrid.RenderSize;

        private int PaintHeight => (int)PaintAreaGrid.RenderSize.Height;
        private int PaintWidth => (int)PaintAreaGrid.RenderSize.Width;

        private bool MoveUp = false;
        private bool MoveDown = false;
        private bool MoveLeft = false;
        private bool MoveRight = false;

        private bool IsGame = true;

        private Axis? XAxis;
        private Axis? YAxis;
        private User? User;
        private Target? Target;
        private Enemy? Enemy;

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

                var anglePoint = Converter.ToAngle_FromRadian(CurrentPos.To2D());

                var wndPoint = Converter.ToWndCoord(
                    new Point2DF(anglePoint.X - Settings.ANGLE_X_SHIFT, anglePoint.Y - Settings.ANGLE_Y_SHIFT));

                return wndPoint;
            }
        }

        private Converter? Converter;

        private int Score = 0;

        /// <summary>
        /// 
        /// </summary>
        public PaintWindow()
        {
            InitializeComponent();

            NetworkThread = new(NetworkReceive);

            MoveTimer = new(Settings.MOVE_TIME);
            MoveTimer.Elapsed += MoveTimerElapsed;

            TargetTimer = new(Random.Next(Settings.TARGET_MIN_TIME, Settings.TARGET_MAX_TIME));
            TargetTimer.Elapsed += TargetTimerElapsed;

            ShotTimer = new(Settings.SHOT_TIME);
            ShotTimer.Elapsed += ShotTimerElapsed;

            Closing += OnClosing;
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            PreviewKeyDown += OnKeyDown;
            PreviewKeyUp += OnKeyUp;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShotTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (Target is not null && User is not null)
            {
                Score += Target.ReceiveShot(User.Shot());
            }

            // lists
            if (User is not null && Enemy is not null)
            {
                Score -= User.ReceiveShot(Enemy.Shot());
            }

            Application.Current.Dispatcher.Invoke(() => Title = $"Score: {Score}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                () => Target?.Move(new(
                    Random.Next(Target.MaxRadius, PaintWidth - Target.MaxRadius * 2),
                    Random.Next(Target.MaxRadius, PaintHeight - Target.MaxRadius * 2)
                    )));

            TargetTimer.Interval = Random.Next(Settings.TARGET_MIN_TIME, Settings.TARGET_MIN_TIME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object? sender, CancelEventArgs e)
        {
            StopGame();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => User?.Move(ShiftedWndPos ?? User.Center));

            // Keyboard
            //Application.Current.Dispatcher.Invoke(() => User?.Move(MoveUp, MoveRight, MoveDown, MoveLeft));

            // lists
            Application.Current.Dispatcher.Invoke(
                () => Enemy?.Follow(User?.Center ?? new(PaintCenterX, PaintCenterY)));
        }

        /// <summary>
        /// 
        /// </summary>
        private void NetworkReceive()
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
                MessageBox.Show("Connection lost");
                Application.Current.Dispatcher.BeginInvoke(new Action(() => Close()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StopGame();

            DrawWindRose();

            DrawPaths();

            ShowStats();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowStats()
        {
            using var userAngleReader = FileReader<float>.Open(Settings.USER_ANG_LOG_FILE, Settings.LOG_SEPARATOR);

            var dataset = new List<Point2DF>();
            foreach (var p in userAngleReader.Get2DPoints())
            {
                dataset.Add(p);
            }

            var mx = Calculator2D.MathExp(dataset);
            var dispersion = Calculator2D.Dispersion(dataset);
            var deviation = Calculator2D.StandartDeviation(dataset);

            MessageBox.Show(
                $"""
                Score: {Score}
                Math expectation: {mx}
                Dispersion: {dispersion}
                Standart deviation: {deviation}
                """);
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawPaths()
        {
            using var userPathReader = FileReader<float>.Open(Settings.USER_ANG_LOG_FILE, Settings.LOG_SEPARATOR);

            var userPath = new Path(userPathReader.Get2DPoints(), PaintSize, new(X_ANGLE_SIZE, Y_ANGLE_SIZE),
                new SolidColorBrush(Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G, Settings.USER_COLOR.B)));

            userPath.Draw(PaintAreaGrid);

            Scalables.Add(userPath);
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawWindRose()
        {
            using var userReader = FileReader<float>.Open(Settings.USER_ANG_LOG_FILE, Settings.LOG_SEPARATOR);

            var userRose = new Graph(userReader.Get2DPoints().Select(p => new PolarPointF(p.X, p.Y)), PaintSize, 
                Brushes.LightGreen);

            userRose.Draw(PaintAreaGrid);

            Scalables.Add(userRose);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopGame()
        {
            IsGame = false;

            NetworkThread.Join();

            MoveTimer.Stop();
            TargetTimer.Stop();
            ShotTimer.Stop();

            UserLogAng.Dispose();
            UserLogWnd.Dispose();
            UserLogCen.Dispose();

            EnemyLogAng.Dispose();
            EnemyLogWnd.Dispose();
            EnemyLogCen.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Converter = new(PaintSize, new(X_ANGLE_SIZE, Y_ANGLE_SIZE));

            XAxis = new(new(0, PaintCenterY), new(PaintWidth, PaintCenterY), PaintSize, Brushes.Black);
            YAxis = new(new(PaintCenterX, 0), new(PaintCenterX, PaintHeight), PaintSize, Brushes.Black);

            User = new(new(SCREEN_INI_CENTER_X, SCREEN_INI_CENTER_Y), Settings.USER_INI_RADIUS, Settings.USER_INI_SPEED,
                new SolidColorBrush(Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G, Settings.USER_COLOR.B)),
                SCREEN_INI_SIZE);
            User.OnShot += UserLogWnd.LogLn;
            User.OnShot += (p) => UserLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            User.OnShot += (p) => UserLogCen.LogLn(Converter?.ToLogCoord(p));

            // add to list of enemies
            Enemy = new(new(Random.Next(Settings.SCREEN_INI_WIDTH), Random.Next(Settings.SCREEN_INI_HEIGHT)),
                Settings.ENEMY_INI_RADIUS, Settings.ENEMY_INI_SPEED,
                new SolidColorBrush(Color.FromRgb((byte)Random.Next(256), (byte)Random.Next(256), (byte)Random.Next(256))),
                SCREEN_INI_SIZE);
            Enemy.OnShot += EnemyLogWnd.LogLn;
            Enemy.OnShot += (p) => EnemyLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            Enemy.OnShot += (p) => EnemyLogCen.LogLn(Converter?.ToLogCoord(p));

            Target = new(new(Random.Next(Settings.SCREEN_INI_WIDTH), Random.Next(Settings.SCREEN_INI_HEIGHT)),
                Settings.TARGET_INI_RADIUS, SCREEN_INI_SIZE);

            Scalables.Add(XAxis); Scalables.Add(YAxis); Scalables.Add(Target); Scalables.Add(User); Scalables.Add(Enemy);
            Scalables.Add(Converter);
            Drawables.Add(XAxis); Drawables.Add(YAxis); Drawables.Add(Target); Drawables.Add(User); Drawables.Add(Enemy);

            foreach (var elem in Drawables)
            {
                elem?.Draw(PaintAreaGrid);
            }

            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintSize);
            }

            NetworkThread.Start();

            MoveTimer.Start();
            TargetTimer.Start();
            ShotTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.Up)
            {
                MoveUp = true;
            }
            if (e.Key == Key.A || e.Key == Key.Left)
            {
                MoveLeft = true;
            }
            if (e.Key == Key.S || e.Key == Key.Down)
            {
                MoveDown = true;
            }
            if (e.Key == Key.D || e.Key == Key.Right)
            {
                MoveRight = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.Up)
            {
                MoveUp = false;
            }
            if (e.Key == Key.A || e.Key == Key.Left)
            {
                MoveLeft = false;
            }
            if (e.Key == Key.S || e.Key == Key.Down)
            {
                MoveDown = false;
            }
            if (e.Key == Key.D || e.Key == Key.Right)
            {
                MoveRight = false;
            }
        }
    }
}