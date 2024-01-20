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
        }

        private void TargetTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                () => Target?.Move(new(
                    Random.Next(Target.MaxRadius, (int)RenderSize.Width - Target.MaxRadius * 2),
                    Random.Next(Target.MaxRadius, (int)RenderSize.Height - Target.MaxRadius * 2)
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
        }

        private void MoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            //User?.Move(Converter?.ToWndCoord(CurrentPos.To2D()) ?? User.Center);
            Application.Current.Dispatcher.Invoke(() => User?.Move(MoveUp, MoveRight, MoveDown, MoveLeft));

            Application.Current.Dispatcher.Invoke(
                () => Enemy?.Follow(User?.Center ?? new((int)RenderSize.Width / 2, (int)RenderSize.Height / 2)));
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
            XAxis = new Axis(new(0, (int)RenderSize.Height / 2), new((int)RenderSize.Width, (int)RenderSize.Height / 2),
                RenderSize, Brushes.Black);
            YAxis = new Axis(new((int)RenderSize.Width / 2, 0), new((int)RenderSize.Width / 2, (int)RenderSize.Height),
                RenderSize, Brushes.Black);

            User = new(new((int)RenderSize.Width / 2, (int)RenderSize.Height / 2), 5, 5, Brushes.Green, RenderSize);
            User.OnShot += UserLogWnd.LogLn;

            Enemy = new(new((int)RenderSize.Width / 2 - 20, (int)RenderSize.Height / 2 - 100), 3, 4, Brushes.Red, RenderSize);
            Enemy.OnShot += UserLogWnd.LogLn;

            Target = new(new(Random.Next((int)RenderSize.Width), Random.Next((int)RenderSize.Height)), 7, RenderSize);

            Converter = new(RenderSize, new(20.0f, 20.0f));

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
                elem?.Scale(RenderSize);
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