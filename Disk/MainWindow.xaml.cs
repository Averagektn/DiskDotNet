using Disk.Calculations.Impl;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Timer = System.Timers.Timer;
using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using PolarPointF = Disk.Data.Impl.PolarPoint<float>;
using PolarPointI = Disk.Data.Impl.PolarPoint<int>;
using Point3DF = Disk.Data.Impl.Point3D<float>;
using Point3DI = Disk.Data.Impl.Point3D<int>;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Size PaintSize => PaintAreaGrid.RenderSize;

        private int PaintHeight => (int)PaintAreaGrid.RenderSize.Height;
        private int PaintWidth => (int)PaintAreaGrid.RenderSize.Width;

        private readonly Timer ShotTimer;
        private readonly Timer MoveTimer;
        private readonly Timer TargetTimer;

        private readonly Thread NetworkThread;

        private bool MoveUp;
        private bool MoveDown;
        private bool MoveLeft;
        private bool MoveRight;
        private bool IsGame;

        private readonly List<IScalable?> Scalables = [];
        private readonly List<IDrawable?> Drawables = [];

        private Axis? XAxis;
        private Axis? YAxis;
        private User? User;
        private Target? Target;
        private readonly Path? Path;
        private readonly Graph? Graph;
        private Enemy? Enemy;

        private readonly Logger UserLogWnd = Logger.GetLogger("userWND.log");
        private readonly Logger UserLogCen = Logger.GetLogger("userCEN.log");
        private readonly Logger UserLogAng = Logger.GetLogger("userANG.log");

        private readonly Logger EnemyLogWnd = Logger.GetLogger("enemyWND.log");
        private readonly Logger EnemyLogCen = Logger.GetLogger("enemyCEN.log");
        private readonly Logger EnemyLogAng = Logger.GetLogger("enemyANG.log");

        private readonly Logger Calculations = Logger.GetLogger("calculations.log");

        // create after logger is closed
        //private readonly FileReader<float> UserPathReader = FileReader<float>.Open("userANG.log", ';');
        //private readonly FileReader<float> EnemyPathReader = FileReader<float>.Open("enemyANG.log", ';');

        private Point3DF? CurrentPos;

        private Converter? Converter;

        private readonly Random Random = new();

        private int Score = 0;

        public MainWindow()
        {
            InitializeComponent();

            NetworkThread = new(NetworkReceive);

            MoveTimer = new(20);
            MoveTimer.Elapsed += MoveTimerElapsed;

            TargetTimer = new(Random.Next(1000, 5000));
            TargetTimer.Elapsed += TargetTimerElapsed;

            ShotTimer = new(20);
            ShotTimer.Elapsed += ShotTimerElapsed;

            Closing += OnClosing;
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            PreviewKeyDown += OnKeyDown;
            PreviewKeyUp += OnKeyUp;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        private void ShotTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (Target is not null && User is not null)
            {
                Score += Target.ReceiveShot(User.Shot());
            }

            if (User is not null && Enemy is not null)
            {
                Score -= User.ReceiveShot(Enemy.Shot());
            }
        }

        private void TargetTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                () => Target?.Move(new(
                    Random.Next(Target.MaxRadius, PaintWidth - Target.MaxRadius * 2),
                    Random.Next(Target.MaxRadius, PaintHeight - Target.MaxRadius * 2)
                    )));

            TargetTimer.Interval = Random.Next(1000, 5000);
        }

        private void OnClosing(object? sender, CancelEventArgs e)
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

            Calculations.Dispose();
        }

        private void MoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            //User?.Move(Converter?.ToWndCoord(CurrentPos.To2D()) ?? User.Center);
            Application.Current.Dispatcher.Invoke(() => User?.Move(MoveUp, MoveRight, MoveDown, MoveLeft));

            Application.Current.Dispatcher.Invoke(
                () => Enemy?.Follow(User?.Center ?? new(PaintWidth / 2, PaintHeight / 2)));
        }

        private void NetworkReceive()
        {
            /*            var con = Connection.GetConnection(IPAddress.Parse("127.0.0.1"), 9888);

                        while (IsGame)
                        {
                            CurrentPos = con.GetXYZ();
                        }*/
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Converter = new(PaintSize, new(40.0f, 40.0f));

            XAxis = new(new(0, PaintHeight / 2), new(PaintWidth, PaintHeight / 2), PaintSize, Brushes.Black);
            YAxis = new(new(PaintWidth / 2, 0), new(PaintWidth / 2, PaintHeight), PaintSize, Brushes.Black);

            User = new(new(PaintWidth / 2, PaintHeight / 2), 5, 5, Brushes.Green, PaintSize);
            User.OnShot += UserLogWnd.LogLn;
            User.OnShot += (p) => UserLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            User.OnShot += (p) => UserLogCen.LogLn(Converter?.ToLogCoord(p));

            Enemy = new(new(Random.Next(PaintWidth), Random.Next(PaintHeight)), 3, 4, Brushes.Red, PaintSize);
            Enemy.OnShot += EnemyLogWnd.LogLn;
            Enemy.OnShot += (p) => EnemyLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            Enemy.OnShot += (p) => EnemyLogCen.LogLn(Converter?.ToLogCoord(p));

            Target = new(new(Random.Next(PaintWidth), Random.Next(PaintHeight)), 7, PaintSize);

            Scalables.Add(XAxis); Scalables.Add(YAxis); Scalables.Add(User); Scalables.Add(Target); Scalables.Add(Enemy);
            Drawables.Add(XAxis); Drawables.Add(YAxis); Drawables.Add(User); Drawables.Add(Target); Drawables.Add(Enemy);

            foreach (var elem in Drawables)
            {
                elem?.Draw(PaintAreaGrid);
            }

            MoveTimer.Start();
            NetworkThread.Start();
            TargetTimer.Start();
            ShotTimer.Start();
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintSize);
            }
        }

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