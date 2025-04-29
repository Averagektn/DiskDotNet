using Disk.Calculations.Impl.Converters;
using Disk.Service.Implementation;
using Disk.ViewModel;
using Disk.Visual.Interface;
using Serilog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View.PaintWindow;

public partial class PaintView : UserControl
{
    private readonly DispatcherTimer ShotTimer;

    private Size PaintPanelSize => PaintRect.RenderSize;

    private IUser User = null!;
    private IProgressTarget Target = null!;

    private Converter Converter => ViewModel.Converter;
    private PaintViewModel ViewModel => (PaintViewModel)DataContext;

    private int PaintPanelCenterX => (int)PaintPanelSize.Width / 2;
    private int PaintPanelCenterY => (int)PaintPanelSize.Height / 2;

    private static Settings Settings => Settings.Default;

    private Point2DI? ShiftedWndPos
    {
        get
        {
            /*            if (ViewModel is null)
                        {
                            return null;
                        }*/

            if (ViewModel.CurrentPos is null)
            {
                return User.Center;
            }
            else
            {
                return Converter.ToWndCoord(new Point2DF(ViewModel.CurrentPos.X - Settings.XAngleShift, ViewModel.CurrentPos.Y - Settings.YAngleShift));
            }
        }
    }

    public PaintView()
    {
        InitializeComponent();

        ShotTimer = new(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(Settings.ShotTime)
        };
        ShotTimer.Tick += ShotTimerElapsed;

        Unloaded += (_, _) => StopGame();
        PaintArea.Loaded += OnLoaded;
        PaintArea.SizeChanged += OnSizeChanged;

        CompositionTarget.Rendering += OnRender;
    }

    private void OnRender(object? sender, EventArgs e)
    {
        if (ShiftedWndPos is not null)
        {
            if (AllowedArea.FillContains(ShiftedWndPos.ToPoint()))
            {
                User.Move(ShiftedWndPos);
            }
            else
            {
                var center = new Point2DI((int)AllowedArea.Bounds.Width / 2, (int)AllowedArea.Bounds.Height / 2);
                var radiusX = AllowedArea.Bounds.Width / 2;
                var radiusY = AllowedArea.Bounds.Height / 2;

                double normalizedX = (ShiftedWndPos.X - center.X) / radiusX;
                double normalizedY = (ShiftedWndPos.Y - center.Y) / radiusY;

                double length = Math.Sqrt((normalizedX * normalizedX) + (normalizedY * normalizedY));

                double scale = 1 / length;
                int nearestX = (int)(center.X + (normalizedX * radiusX * scale));
                int nearestY = (int)(center.Y + (normalizedY * radiusY * scale));

                User.Move(new(nearestX, nearestY));
            }
        }
    }

    private List<Point2DI> GetMultipleShots()
    {
        var shot = User.Shot();
        //_ = shot.X;
        //_ = shot.Y;
        //_ = User.Radius / 2;
        //_ = Math.Sqrt(2);

        return [shot];
        /*        return [new(x - halfRadius, y), new(x + halfRadius, y), new(x, y - halfRadius), new(x, y + halfRadius),
                        new((int)(x - (halfRadius / sqrt2)), (int)(y - (halfRadius / sqrt2))),
                        new((int)(x + (halfRadius / sqrt2)), (int)(y - (halfRadius / sqrt2))),
                        new((int)(x - (halfRadius / sqrt2)), (int)(y + (halfRadius / sqrt2))),
                        new((int)(x + (halfRadius / sqrt2)), (int)(y + (halfRadius / sqrt2)))];*/
    }

    private void ShotTimerElapsed(object? sender, EventArgs e)
    {
        var shots = GetMultipleShots();

        var shot = User.Center;
        int shotScore = 0;
        for (int i = 0; i < shots.Count && shotScore == 0; i++)
        {
            shotScore = Target.ReceiveShot(shots[i]);
        }

        var angleShot = Converter.ToAngle_FromWnd(shot);

        // pit
        bool isPathInTargetStarts = shotScore != 0 && ViewModel.IsPathToTarget;
        if (isPathInTargetStarts)
        {
            ViewModel.SwitchToPathInTarget(shot);
        }

        //bool isValidShot = angleShot.X != 0 && angleShot.Y != 0;
        bool isPathInTarget = !ViewModel.IsPathToTarget;
        //if (isValidShot)
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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        User = DrawableFabric.GetIniUser(Settings.CursorFilePath, PaintArea);
        User.OnShot += (p) => ViewModel.FullPath.Add(Converter.ToAngle_FromWnd(p));

        var center = ViewModel.NextTargetCenter ?? new(0, 0);
        var converter = DrawableFabric.GetIniConverter();
        var wndCenter = converter.ToWndCoord(center);
        Target = DrawableFabric.GetIniProgressTarget(Settings.TargetFilePath, wndCenter, PaintArea);
        Target.OnReceiveShot += shot => ViewModel.Score += shot;

        if (ViewModel.IsGame)
        {
            ViewModel.StartReceiving();

            Target.Draw();
            User.Draw();

            ShotTimer.Start();
        }
    }

    private void StopGame()
    {
        ShotTimer.Stop();
        CompositionTarget.Rendering -= OnRender;

        Target.Remove();
        User.Remove();

        User.ClearOnShot();
    }

    private void OnSizeChanged(object sender, RoutedEventArgs e)
    {
        AllowedArea.RadiusX = PaintPanelCenterX;
        AllowedArea.RadiusY = PaintPanelCenterY;
        AllowedArea.Center = new(PaintPanelCenterX, PaintPanelCenterY);

        Converter.Scale(new((int)PaintArea.ActualWidth, (int)PaintArea.ActualHeight));
    }

    private void OnStopClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsPathToTarget)
        {
            ViewModel.SavePathToTarget(User.Center);
        }
        else
        {
            ViewModel.SavePathInTarget(Target);
        }
        StopGame();

        _ = Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            await ViewModel.SaveAttemptResultAsync();
        }).Task.ContinueWith(task =>
        {
            if (task.Exception is not null)
            {
                Log.Fatal($"{task.Exception.Message} \n {task.Exception.StackTrace}");
            }
        });
    }
}
