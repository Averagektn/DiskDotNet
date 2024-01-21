using Disk.Calculations.Impl;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point3DF = Disk.Data.Impl.Point3D<float>;
using PolarPointF = Disk.Data.Impl.PolarPoint<float>;
using Timer = System.Timers.Timer;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PaintWindow : Window
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
        private Enemy? Enemy;

        private readonly Logger UserLogWnd = Logger.GetLogger("userWND.log");
        private readonly Logger UserLogCen = Logger.GetLogger("userCEN.log");
        private readonly Logger UserLogAng = Logger.GetLogger("userANG.log");

        private readonly Logger EnemyLogWnd = Logger.GetLogger("enemyWND.log");
        private readonly Logger EnemyLogCen = Logger.GetLogger("enemyCEN.log");
        private readonly Logger EnemyLogAng = Logger.GetLogger("enemyANG.log");

        private Point3DF? CurrentPos;

        private Converter? Converter;

        private readonly Random Random = new();

        private int Score = 0;

        public PaintWindow()
        {
            InitializeComponent();

            NetworkThread = new(NetworkReceive);

            MoveTimer = new(20);
            MoveTimer.Elapsed += MoveTimerElapsed;

            TargetTimer = new(Random.Next(1000, 5000));
            TargetTimer.Elapsed += TargetTimerElapsed;

            ShotTimer = new(200);
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

            Application.Current.Dispatcher.Invoke(() => Title = $"Score: {Score}");
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
            StopGame();
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
            StopGame();

            DrawWindRose();

            DrawPaths();

            ShowStats();
        }

        private static void ShowStats()
        {
            using var userAngleReader = FileReader<float>.Open("userANG.log", ';');

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
                Math expectation: {mx}
                Dispersion: {dispersion}
                Standart deviation: {deviation}
                """);
        }

        private void DrawPaths()
        {
            using var userPathReader = FileReader<float>.Open("userANG.log", ';');
            using var enemyPathReader = FileReader<float>.Open("enemyANG.log", ';');

            var userPath = new Path(userPathReader.Get2DPoints(), PaintSize, new(40.0f, 40.0f), Brushes.Green);
            var enemyPath = new Path(enemyPathReader.Get2DPoints(), PaintSize, new(40.0f, 40.0f), Brushes.DarkRed);

            userPath.Draw(PaintAreaGrid);
            enemyPath.Draw(PaintAreaGrid);

            Scalables.Add(userPath);
            Scalables.Add(enemyPath);
        }

        private void DrawWindRose()
        {
            using var userReader = FileReader<float>.Open("userANG.log", ';');

            var userRose = new Graph(userReader.Get2DPoints().Select(p => new PolarPointF(p.X, p.Y)),
                PaintSize, Brushes.LightGreen);

            userRose.Draw(PaintAreaGrid);

            Scalables.Add(userRose);
        }

        private void ClearWnd()
        {
            PaintAreaGrid.Children.Clear();
        }

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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Converter = new(PaintSize, new(40.0f, 40.0f));

            XAxis = new(new(0, PaintHeight / 2), new(PaintWidth, PaintHeight / 2), PaintSize, Brushes.Black);
            YAxis = new(new(PaintWidth / 2, 0), new(PaintWidth / 2, PaintHeight), PaintSize, Brushes.Black);

            User = new(new(PaintWidth / 2, PaintHeight / 2), 5, 5, Brushes.Green, PaintSize);
            User.OnShot += UserLogWnd.LogLn;
            User.OnShot += (p) => UserLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            User.OnShot += (p) => UserLogCen.LogLn(Converter?.ToLogCoord(p));

            Enemy = new(new(Random.Next(PaintWidth), Random.Next(PaintHeight)), 3, 4, Brushes.DarkRed, PaintSize);
            Enemy.OnShot += EnemyLogWnd.LogLn;
            Enemy.OnShot += (p) => EnemyLogAng.LogLn(Converter?.ToAngle_FromWnd(p));
            Enemy.OnShot += (p) => EnemyLogCen.LogLn(Converter?.ToLogCoord(p));

            Target = new(new(Random.Next(PaintWidth), Random.Next(PaintHeight)), 7, PaintSize);

            Scalables.Add(XAxis); Scalables.Add(YAxis); Scalables.Add(Target); Scalables.Add(User); Scalables.Add(Enemy);
            Scalables.Add(Converter);
            Drawables.Add(XAxis); Drawables.Add(YAxis); Drawables.Add(Target); Drawables.Add(User); Drawables.Add(Enemy);

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