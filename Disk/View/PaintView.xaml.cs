using Disk.Calculations.Impl.Converters;
using Disk.ViewModel;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View.PaintWindow
{
    /// <summary>
    /// Interaction logic for PaintWindowView.xaml
    /// </summary>
    public partial class PaintView : UserControl
    {
        private User User = null!;
        private ProgressTarget Target = null!;
        private Converter Converter => ViewModel.Converter;

        private PaintViewModel ViewModel => (PaintViewModel)DataContext;

        private readonly DispatcherTimer ShotTimer;
        private readonly DispatcherTimer MoveTimer;

        private readonly List<IScalable?> Scalables = [];
        private readonly List<IDrawable?> Drawables = [];

        private Point2DI? ShiftedWndPos
        {
            get => ViewModel.CurrentPos is null
                ? User.Center
                : Converter.ToWndCoord(new Point2DF(ViewModel.CurrentPos.X - Settings.XAngleShift, ViewModel.CurrentPos.Y - Settings.YAngleShift));
        }

        private Size PaintPanelSize => PaintRect.RenderSize;
        private int PaintPanelCenterX => (int)PaintPanelSize.Width / 2;
        private int PaintPanelCenterY => (int)PaintPanelSize.Height / 2;

        private static Settings Settings => Settings.Default;

        public PaintView()
        {
            InitializeComponent();

            MoveTimer = new(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.MoveTime)
            };
            MoveTimer.Tick += MoveTimerElapsed;

            ShotTimer = new(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.ShotTime)
            };
            ShotTimer.Tick += ShotTimerElapsed;

            Unloaded += (_, _) => StopGame();
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        private void ShotTimerElapsed(object? sender, EventArgs e)
        {
            var shot = User.Shot();
            var shotScore = Target.ReceiveShot(shot);
            var angleShot = Converter.ToAngle_FromWnd(shot);

            // pit
            bool isPathInTargetStarts = shotScore != 0 && ViewModel.IsPathToTarget;
            if (isPathInTargetStarts)
            {
                ViewModel.SwitchToPathInTarget(shot);
            }

            bool isValidShot = angleShot.X != 0 && angleShot.Y != 0;
            bool isPathInTarget = !ViewModel.IsPathToTarget;
            if (isValidShot)
            {
                if (isPathInTarget)
                {
                    ViewModel.PathsInTargets[ViewModel.TargetId - 1].Add(angleShot);
                }
                else
                {
                    ViewModel.PathsToTargets[ViewModel.TargetId - 1].Add(angleShot);
                }
            }

            // ptt
            bool isPathToTargetStarts = Target.IsFull;
            if (isPathToTargetStarts)
            {
                if (!ViewModel.SwitchToPathToTarget(Target))
                {
                    OnStopClick(this, new());
                }
            }
        }

        private void MoveTimerElapsed(object? sender, EventArgs e)
        {
            if (ShiftedWndPos is not null && AllowedArea.FillContains(ShiftedWndPos.ToPoint()))
            {
                User.Move(ShiftedWndPos);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            User = ViewModel.GetUser();
            Target = ViewModel.GetProgressTarget();

            Drawables.Add(Target); Drawables.Add(User);
            Scalables.Add(Target); Scalables.Add(User); Scalables.Add(Converter);

            foreach (var elem in Drawables)
            {
                elem?.Draw(PaintArea);
            }
            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintPanelSize);
            }

            MoveTimer.Start();
            ShotTimer.Start();
        }

        private void StopGame()
        {
            Target.Remove(PaintArea.Children);
            User.Remove(PaintArea.Children);

            _ = Drawables.Remove(Target);
            _ = Scalables.Remove(Target);

            _ = Drawables.Remove(User);
            _ = Scalables.Remove(User);

            User.ClearOnShot();

            MoveTimer.Stop();
            ShotTimer.Stop();
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            AllowedArea.RadiusX = PaintPanelCenterX;
            AllowedArea.RadiusY = PaintPanelCenterY;
            AllowedArea.Center = new(PaintPanelCenterX, PaintPanelCenterY);

            foreach (var elem in Scalables)
            {
                elem?.Scale(PaintPanelSize);
            }
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            StopGame();
            ViewModel.SaveSessionResult();
        }

        private void CbTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PaintArea.Children.Clear();

            if (ViewModel is not null)
            {
                var figures = ViewModel.GetPathAndRose(Target, PaintPanelSize);
                foreach (var figure in figures)
                {
                    Scalables.Add(figure);

                    figure.Draw(PaintArea);
                    figure.Scale(PaintPanelSize);
                }
            }
        }
    }
}
