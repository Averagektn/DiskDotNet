using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using Disk.Calculations.Implementations.Converters;
using Disk.Services.Implementations;
using Disk.ViewModels;
using Disk.Visual.Implementations;
using Disk.Visual.Interfaces;

using Serilog;

using Point2DF = Disk.Data.Impl.Point2D<float>;
using Point2DI = Disk.Data.Impl.Point2D<int>;
using Settings = Disk.Properties.Config.Config;

namespace Disk.Views.PaintWindow;

public partial class PaintView : UserControl
{
    private readonly DispatcherTimer ShotTimer;

    private Size PaintPanelSize => PaintRect.RenderSize;

    private Cursor PaintCursor = null!;
    private IProgressTarget Target = null!;

    private Converter? Converter => ViewModel?.Converter;
    private PaintViewModel? ViewModel => DataContext as PaintViewModel;

    private int PaintPanelCenterX => (int)PaintPanelSize.Width / 2;
    private int PaintPanelCenterY => (int)PaintPanelSize.Height / 2;

    private static Settings Settings => Settings.Default;

    private Point2DI? ShiftedWndPos
    {
        get
        {
            if (ViewModel?.CurrentPos is null)
            {
                return PaintCursor.Center;
            }
            else
            {
                var angleShiftedPoint = new Point2DF(ViewModel.CurrentPos.X - Settings.XAngleShift, ViewModel.CurrentPos.Y - Settings.YAngleShift);

                return Converter?.ToWndCoord(angleShiftedPoint);
            }
        }
    }

    public PaintView()
    {
        InitializeComponent();

        ShotTimer = new(DispatcherPriority.Normal)
        {
            // 60HZ max
            Interval = TimeSpan.FromMilliseconds(Settings.ShotTime)
        };
        ShotTimer.Tick += ShotTimerElapsed;

        PaintArea.Loaded += OnLoaded;
        PaintArea.SizeChanged += OnSizeChanged;
        PaintArea.Unloaded += (_, _) => StopGame();

        CompositionTarget.Rendering += OnRender;
    }

    private void OnRender(object? sender, EventArgs e)
    {
        if (ShiftedWndPos is not null)
        {
            if (AllowedArea.FillContains(ShiftedWndPos.ToPoint()))
            {
                PaintCursor.MoveSmooth(ShiftedWndPos);
            }
            else
            {
                var center = new Point2DI((int)AllowedArea.Bounds.Width / 2, (int)AllowedArea.Bounds.Height / 2);
                double radiusX = AllowedArea.Bounds.Width / 2;
                double radiusY = AllowedArea.Bounds.Height / 2;

                double normalizedX = (ShiftedWndPos.X - center.X) / radiusX;
                double normalizedY = (ShiftedWndPos.Y - center.Y) / radiusY;

                double length = Math.Sqrt((normalizedX * normalizedX) + (normalizedY * normalizedY));

                double scale = 1 / length;
                int nearestX = (int)(center.X + (normalizedX * radiusX * scale));
                int nearestY = (int)(center.Y + (normalizedY * radiusY * scale));

                PaintCursor.MoveSmooth(new(nearestX, nearestY));
            }
        }
    }

    private List<Point2DI> GetMultipleShots()
    {
        Point2DI shot = PaintCursor.Shot();
        /*        int x = shot.X;
                int y = shot.Y;
                int halfRadius = Cursor.Radius / 2;
                double sqrt2 = Math.Sqrt(2);*/

        return [shot];
        /*        return [new(x - halfRadius, y), new(x + halfRadius, y), new(x, y - halfRadius), new(x, y + halfRadius),
                                new((int)(x - (halfRadius / sqrt2)), (int)(y - (halfRadius / sqrt2))),
                                new((int)(x + (halfRadius / sqrt2)), (int)(y - (halfRadius / sqrt2))),
                                new((int)(x - (halfRadius / sqrt2)), (int)(y + (halfRadius / sqrt2))),
                                new((int)(x + (halfRadius / sqrt2)), (int)(y + (halfRadius / sqrt2)))];*/
    }

    private void ShotTimerElapsed(object? sender, EventArgs e)
    {
        if (ViewModel is null || Converter is null || !ViewModel.IsGame)
        {
            return;
        }

        List<Point2DI> shots = GetMultipleShots();

        Point2DI shot = PaintCursor.Center;
        int shotScore = 0;
        for (int i = 0; i < shots.Count && shotScore == 0; i++)
        {
            shotScore = Target.ReceiveShot(shots[i]);
        }

        _ = Converter.ToAngle_FromWnd(shot);

        // pit
        bool isPathInTargetStarts = shotScore != 0 && ViewModel.IsPathToTarget;
        if (isPathInTargetStarts)
        {
            ViewModel.SwitchToPathInTarget();
        }

        /*        if (ViewModel.IsPathToTarget)
                {
                    ViewModel.PathsToTargets[ViewModel.TargetId].Add(ViewModel.CurrentPos ?? angleShot);
                }
                else
                {
                    ViewModel.PathsInTargets[ViewModel.TargetId].Add(ViewModel.CurrentPos ?? angleShot);
                }*/

        // ptt
        bool isPathToTargetStarts = Target.IsFull;
        if (isPathToTargetStarts && !ViewModel.SwitchToPathToTarget(Target))
        {
            OnStopClick(sender, e);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null || Converter is null)
        {
            return;
        }

        PaintCursor = DrawableFabric.GetIniCursor(Settings.CursorFilePath, PaintArea);
        //PaintCursor.OnShot += (p) => ViewModel.FullPath.Add(Converter.ToAngle_FromWnd(p));

        Target = DrawableFabric.GetIniProgressTarget(Settings.TargetFilePath, new(-1000, -1000), PaintArea);
        Target.OnReceiveShot += shot =>
        {
            ViewModel.Score += shot;
            ViewModel.ShotsCount++;
            if (shot != 0)
            {
                ViewModel.HitsCount++;
            }
        };

        Point2DI center = ViewModel.TargetCenter ?? new(0, 0);
        Target.Move(center);

        ViewModel.StartReceiving();

        Target.Draw();
        PaintCursor.Draw();

        ShotTimer.Start();
    }

    private void StopGame()
    {
        ShotTimer.Stop();
        CompositionTarget.Rendering -= OnRender;

        Target.Remove();
        PaintCursor.Remove();

        PaintCursor.ClearOnShot();
    }

    private void OnSizeChanged(object sender, RoutedEventArgs e)
    {
        if (Converter is null)
        {
            return;
        }

        AllowedArea.RadiusX = PaintPanelCenterX;
        AllowedArea.RadiusY = PaintPanelCenterY;
        AllowedArea.Center = new(PaintPanelCenterX, PaintPanelCenterY);

        Converter.Scale(new((int)PaintArea.ActualWidth, (int)PaintArea.ActualHeight));
    }

    private void OnStopClick(object? sender, EventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        StopGame();

        _ = Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            ViewModel.StopRecord();
            await ViewModel.ShowResultAsync();
        }).Task.ContinueWith(task =>
        {
            if (task.Exception is not null)
            {
                Log.Error($"{task.Exception.Message} \n {task.Exception.StackTrace}");
            }
        });
    }
}
