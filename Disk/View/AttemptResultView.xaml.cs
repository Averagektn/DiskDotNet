using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Service.Implementation;
using Disk.ViewModel;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View;

public partial class AttemptResultView : UserControl
{
    private DispatcherTimer? MoveTimer;
    private IUser? _user;
    private ITarget? _target;
    private bool _isCursorVisible = true;
    private bool _isTargetVisible = true;
    private int _currentIndex = -1;

    private List<IStaticFigure> _pathAndRose = [];
    private readonly Size IniScreenSize = new(Settings.IniScreenWidth, Settings.IniScreenHeight);
    private readonly List<IStaticFigure> _pathToTargetEllipses = [];
    private readonly List<IStaticFigure> _pathInTargetEllipses = [];
    private readonly List<IStaticFigure> _fullPathEllipses = [];

    private static readonly Settings Settings = Settings.Default;

    private AttemptResultViewModel? ViewModel => DataContext as AttemptResultViewModel;
    private Size PaintPanelSize => PaintArea.RenderSize;
    private Converter? Converter => ViewModel?.Converter;


    private bool _isReply;
    private bool IsReply
    {
        get => _isReply;
        set
        {
            _isReply = value;
            if (ViewModel is not null)
            {
                ViewModel.IsStopEnabled = value;
            }
        }
    }

    public AttemptResultView()
    {
        InitializeComponent();

        SizeChanged += OnSizeChanged;

        Loaded += OnSizeChanged;
        Loaded += SelectionChanged;
        Loaded += OnLoad;

        Unloaded += StopTimer;
    }

    private void OnLoad(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }
    }

    private void OnSizeChanged(object sender, RoutedEventArgs e)
    {
        Converter?.Scale(PaintPanelSize);
    }

    private void SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null || Converter is null)
        {
            return;
        }

        _pathAndRose.ForEach(p => p.Remove());
        // FIX GRAPH AREA
        _pathAndRose = ViewModel.GetPathAndRose(PathArea);
        _pathAndRose.ForEach(p => p.Draw());

        if (_currentIndex == ViewModel.SelectedIndex)
        {
            return;
        }
        _currentIndex = ViewModel.SelectedIndex;

        _target?.Remove();
        _user?.Remove();
        _user ??= DrawableFabric.GetIniUser(string.Empty, PaintArea);
        _target ??= DrawableFabric.GetIniProgressTarget(string.Empty, new(0, 0), PaintArea);
        if (_isTargetVisible)
        {
            _target.Draw();
        }
        if (_isCursorVisible)
        {
            _user.Draw();
        }
        if (!IsReply)
        {
            _user.Move(ViewModel.UserCenter);
            _target.Move(ViewModel.TargetCenter);
        }

        if (ViewModel.SelectedIndex >= 0)
        {
            var allEmpty = _fullPathEllipses.Count == 0 && _pathInTargetEllipses.Count == 0 && _pathToTargetEllipses.Count == 0;
            if (_isFullPathEllipseVisible || allEmpty)
            {
                RecalculateEllipse(_fullPathEllipses, [.. ViewModel.CurrPathToTarget, .. ViewModel.CurrPathInTarget],
                    Brushes.MediumPurple, Brushes.Purple);
            }
            if (_isPathToTargetEllipseVisible || allEmpty)
            {
                RecalculateEllipse(_pathToTargetEllipses, [.. ViewModel.CurrPathToTarget], Brushes.LawnGreen, Brushes.Green);
            }
            if (_isPathInTargetEllipseVisible || allEmpty)
            {
                RecalculateEllipse(_pathInTargetEllipses, [.. ViewModel.CurrPathInTarget], Brushes.CadetBlue, Brushes.Blue);
            }
        }
    }

    private void RecalculateEllipse(List<IStaticFigure> ellipseList, List<Point2D<float>> points, Brush fill, Brush bound)
    {
        ellipseList.ForEach(e => e.Remove());
        ellipseList.Clear();

        var convexHull = CreateConvexHull(points, bound, fill);
        var boundingEllipse = CreateBoundingEllipse(points, bound);

        ellipseList.AddRange([convexHull, boundingEllipse]);
    }

    private IStaticFigure CreateConvexHull(List<Point2D<float>> points, Brush borderColor, Brush fillColor)
    {
        var converter = DrawableFabric.GetIniConverter();
        var hullPoints = ConvexHull.GetConvexHull(points)
            .Select(p => new Point2D<int>(converter.ToWndCoordX(p.X), converter.ToWndCoordY(p.Y)))
            .ToList();
        var ch = new ConvexHull(hullPoints, borderColor, fillColor, EllipseArea, IniScreenSize);
        ch.Scale();

        return ch;
    }

    private IStaticFigure CreateBoundingEllipse(List<Point2D<float>> points, Brush color)
    {
        var converter = DrawableFabric.GetIniConverter();
        var ellipse = new BoundingEllipse([.. points.Select(converter.ToWndCoord)], color, EllipseArea, IniScreenSize);
        ellipse.Scale();

        return ellipse;
    }

    private bool _isPathToTargetEllipseVisible = false;
    public void ShowPathToTargetEllipse(object sender, RoutedEventArgs e)
    {
        if (!_isPathToTargetEllipseVisible)
        {
            _pathToTargetEllipses.ForEach(e =>
            {
                e.Scale();
                e.Draw();
            });
            _isPathToTargetEllipseVisible = true;
        }
    }

    public void HidePathToTargetEllipse(object sender, RoutedEventArgs e)
    {
        if (_isPathToTargetEllipseVisible)
        {
            _pathToTargetEllipses.ForEach(e => e.Remove());
            _isPathToTargetEllipseVisible = false;
        }
    }

    private bool _isPathInTargetEllipseVisible = false;
    public void ShowPathInTargetEllipse(object sender, RoutedEventArgs e)
    {
        if (!_isPathInTargetEllipseVisible)
        {
            _pathInTargetEllipses.ForEach(e =>
            {
                e.Scale();
                e.Draw();
            });
            _isPathInTargetEllipseVisible = true;
        }
    }

    public void HidePathInTargetEllipse(object sender, RoutedEventArgs e)
    {
        if (_isPathInTargetEllipseVisible)
        {
            _pathInTargetEllipses.ForEach(e => e.Remove());
            _isPathInTargetEllipseVisible = false;
        }
    }

    private bool _isFullPathEllipseVisible = false;
    public void ShowFullPathEllipse(object sender, RoutedEventArgs e)
    {
        if (!_isFullPathEllipseVisible)
        {
            _fullPathEllipses.ForEach(e =>
            {
                e.Scale();
                e.Draw();
            });
            _isFullPathEllipseVisible = true;
        }
    }

    public void HideFullPathEllipse(object sender, RoutedEventArgs e)
    {
        if (_isFullPathEllipseVisible)
        {
            _fullPathEllipses.ForEach(e => e.Remove());
            _isFullPathEllipseVisible = false;
        }
    }

    private void ShowCursor(object sender, RoutedEventArgs e)
    {
        if (!_isCursorVisible)
        {
            _isCursorVisible = true;
            _user?.Draw();
        }
    }

    private void HideCursor(object sender, RoutedEventArgs e)
    {
        if (_isCursorVisible)
        {
            _isCursorVisible = false;
            _user?.Remove();
        }
    }

    private void ShowTarget(object sender, RoutedEventArgs e)
    {
        if (!_isTargetVisible)
        {
            _isTargetVisible = true;
            _target?.Draw();
        }
    }

    private void HideTarget(object sender, RoutedEventArgs e)
    {
        if (_isTargetVisible)
        {
            _isTargetVisible = false;
            _target?.Remove();
        }
    }

    private void ReplyClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null || _user is null || _target is null)
        {
            return;
        }

        IsReply = true;

        var selectedIndex = ViewModel.SelectedIndex;
        var enumerator = ViewModel.FullPath.GetEnumerator();
        MoveTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(ViewModel.CurrentAttempt.SamplingInterval)
        };
        MoveTimer.Tick += (_, _) =>
        {
            if (enumerator.MoveNext() && IsReply)
            {
                _user.Move(ViewModel.Converter.ToWndCoord(enumerator.Current.Point));

                if (enumerator.Current.IsNewTarget && ++selectedIndex < ViewModel.TargetCenters.Count)
                {
                    _target.Move(ViewModel.Converter.ToWndCoord(ViewModel.TargetCenters[selectedIndex]));
                    ViewModel.SelectedIndex = selectedIndex;
                }
            }
            else
            {
                StopTimer(sender, e);
            }
        };

        MoveTimer.Start();
    }

    private void StopTimer(object sender, RoutedEventArgs e)
    {
        if (MoveTimer is not null && MoveTimer.IsEnabled)
        {
            IsReply = false;
            MoveTimer.Stop();
        }
    }
}
