using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        private bool MoveUp;
        private bool MoveDown;
        private bool MoveLeft;
        private bool MoveRight;

        private readonly List<IScalable?> Scalables = [];
        private readonly List<IDrawable?> Drawables = [];

        private Axis? XAxis;
        private Axis? YAxis;
        private User? User;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            PreviewKeyDown += OnKeyDown;
            PreviewKeyUp += OnKeyUp;
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            XAxis = new Axis(new(0, (int)RenderSize.Height / 2), new((int)RenderSize.Width, (int)RenderSize.Height / 2), 
                RenderSize, Brushes.Black);
            YAxis = new Axis(new((int)RenderSize.Width / 2, 0), new((int)RenderSize.Width / 2, (int)RenderSize.Height), 
                RenderSize, Brushes.Black);
            User = new(new((int)RenderSize.Width / 2, (int)RenderSize.Height / 2), 5, 5, Brushes.Green, RenderSize);

            Scalables.Add(XAxis); Scalables.Add(YAxis); Scalables.Add(User);
            Drawables.Add(XAxis); Drawables.Add(YAxis); Drawables.Add(User);

            foreach (var elem in Drawables)
            {
                elem?.Draw(PaintAreaGrid);
            }
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            foreach(var elem in Scalables)
            {
                elem?.Scale(RenderSize);
            }
        }
    }
}