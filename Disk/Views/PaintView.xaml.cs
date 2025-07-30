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

    private Point2DI ShiftedWndPos
    {
        get
        {
            if (ViewModel?.CurrentPos is null)
            {
                return PaintCursor.Center;
            }
            else
            {
                float x = ViewModel.CurrentPos.X - Settings.XAngleShift;
                float y = ViewModel.CurrentPos.Y - Settings.YAngleShift;
                var angleShiftedPoint = new Point2DF(x, y);

                return Converter?.ToWndCoord(angleShiftedPoint) ?? new();
            }
        }
    }

    public PaintView()
    {
        InitializeComponent();

        ShotTimer = new(DispatcherPriority.Send)
        {
            Interval = TimeSpan.FromMilliseconds(Settings.TargetRedrawMs)
        };
        ShotTimer.Tick += ShotTimerElapsed;

        PaintArea.Loaded += OnLoaded;
        PaintArea.SizeChanged += OnSizeChanged;
        PaintArea.Unloaded += (_, _) => StopGame();

        CompositionTarget.Rendering += OnRender;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null || Converter is null)
        {
            return;
        }

        PaintCursor = DrawableFabric.GetIniCursor(Settings.CursorFilePath, PaintArea);

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
                Point2DI neearestPoint = GetNearestPoint();
                PaintCursor.MoveSmooth(neearestPoint);
            }

            Target.UpdateProgress();
        }
    }

    private Point2DI GetNearestPoint()
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

        return new Point2DI(nearestX, nearestY);
    }

    private void ShotTimerElapsed(object? sender, EventArgs e)
    {
        if (ViewModel is null || Converter is null || !ViewModel.IsGame)
        {
            return;
        }

        List<Point2DI> shots = GetShots();

        int shotScore = 0;
        for (int i = 0; i < shots.Count && shotScore == 0; i++)
        {
            shotScore = Target.ReceiveShot(shots[i]);
        }

        // pit
        bool isPathInTargetStarts = shotScore != 0 && ViewModel.IsPathToTarget;
        if (isPathInTargetStarts)
        {
            ViewModel.SwitchToPathInTarget();
        }

        // ptt
        bool isPathToTargetStarts = Target.IsFull;
        if (isPathToTargetStarts && !ViewModel.SwitchToPathToTarget(Target))
        {
            OnStopClick(sender, e);
        }
    }

    private List<Point2DI> GetShots()
    {
        Point2DI shot = PaintCursor.Shot();

        return [shot];
    }

    private void StopGame()
    {
        ShotTimer.Stop();
        CompositionTarget.Rendering -= OnRender;

        Target.Remove();
        PaintCursor.Remove();

        PaintCursor.ClearOnShot();
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
}
