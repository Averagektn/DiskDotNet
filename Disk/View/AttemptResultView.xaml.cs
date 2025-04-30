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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View;

public partial class AttemptResultView : UserControl
{
    private IUser? _user;
    private ITarget? _target;
    private int _currentIndex = -1;

    private List<IStaticFigure> _pathAndRose = [];
    private readonly List<IStaticFigure> _pathToTargetEllipses = [];
    private readonly List<IStaticFigure> _pathInTargetEllipses = [];
    private readonly List<IStaticFigure> _fullPathEllipses = [];
    private readonly Size _iniScreenSize = new(_settings.IniScreenWidth, _settings.IniScreenHeight);

    private static readonly Settings _settings = Settings.Default;

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

        SizeChanged += (_, _) => Converter?.Scale(PaintPanelSize);

        Loaded += SelectionChanged;
        Loaded += (_, _) => SidebarTransform.X = Sidebar.ActualWidth + 100;

        Unloaded += StopReply;
        CompositionTarget.Rendering += OnRender;
    }


    private void SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null || Converter is null)
        {
            return;
        }

        _pathAndRose.ForEach(p => p.Remove());
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

        var allEmpty = _fullPathEllipses.Count == 0 && _pathInTargetEllipses.Count == 0 && _pathToTargetEllipses.Count == 0;
        if (ViewModel.CurrPathInTarget.Count > 5 && ViewModel.CurrPathToTarget.Count != 5 && (_isFullPathEllipseVisible || allEmpty))
        {
            RecalculateEllipse(_fullPathEllipses, [.. ViewModel.CurrPathToTarget, .. ViewModel.CurrPathInTarget],
                Brushes.MediumPurple, Brushes.Purple);
        }
        if (ViewModel.CurrPathInTarget.Count > 5 && (_isPathInTargetEllipseVisible || allEmpty))
        {
            RecalculateEllipse(_pathInTargetEllipses, [.. ViewModel.CurrPathInTarget], Brushes.CadetBlue, Brushes.Blue);
        }
        if (ViewModel.CurrPathToTarget.Count > 5 && (_isPathToTargetEllipseVisible || allEmpty))
        {
            RecalculateEllipse(_pathToTargetEllipses, [.. ViewModel.CurrPathToTarget], Brushes.LawnGreen, Brushes.Green);
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

    #region Ellipse and ConvexHull creation
    private IStaticFigure CreateConvexHull(List<Point2D<float>> points, Brush borderColor, Brush fillColor)
    {
        ConvexHull ch;

        try
        {
            var converter = DrawableFabric.GetIniConverter();
            ch = new ConvexHull([.. points.Select(converter.ToWndCoord)], borderColor, fillColor, EllipseArea, _iniScreenSize);
            ch.Scale();
        }
        catch
        {
            ch = new ConvexHull([], Brushes.Transparent, Brushes.Transparent, EllipseArea, _iniScreenSize);
        }

        return ch;
    }

    private IStaticFigure CreateBoundingEllipse(List<Point2D<float>> points, Brush color)
    {
        var converter = DrawableFabric.GetIniConverter();
        BoundingEllipse ellipse;

        try
        {
            ellipse = new BoundingEllipse([.. points.Select(converter.ToWndCoord)], color, EllipseArea, _iniScreenSize);
            ellipse.Scale();
        }
        catch
        {
            ellipse = new BoundingEllipse([], color, EllipseArea, _iniScreenSize);
        }

        return ellipse;
    }
    #endregion

    #region Reply

    private int _selectedIndex;
    private IEnumerator<(bool IsNewTarget, Point2D<float> Point)>? _enumerator;
    private Point2D<int>? _replyCenter;
    private DispatcherTimer? _coordTimer;

    private void OnRender(object? sender, EventArgs e)
    {
        if (!IsReply || ViewModel is null || _user is null || _target is null)
        {
            return;
        }

        // _enumerator is always not null. See ReplyClick method
        if (_replyCenter is not null && IsReply)
        {
            _user.Move(_replyCenter);

            if (_enumerator!.Current.IsNewTarget && ++_selectedIndex < ViewModel.TargetCenters.Count)
            {
                _target.Move(ViewModel.Converter.ToWndCoord(ViewModel.TargetCenters[_selectedIndex]));
                ViewModel.SelectedIndex = _selectedIndex;
            }
        }
    }


    private void StopReply(object sender, RoutedEventArgs e)
    {
        _coordTimer?.Stop();
        CompositionTarget.Rendering -= OnRender;
        IsReply = false;
    }

    private void ReplyClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        _selectedIndex = ViewModel.SelectedIndex;
        _enumerator = ViewModel.FullPath.GetEnumerator();
        IsReply = true;

        _coordTimer = new(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(ViewModel.CurrentAttempt.SamplingInterval)
        };
        _coordTimer.Tick += (_, _) =>
        {
            if (ViewModel is null)
            {
                return;
            }

            if (_enumerator!.MoveNext())
            {
                _replyCenter = ViewModel.Converter.ToWndCoord(_enumerator.Current.Point);
            }
            else
            {
                IsReply = false;
                _replyCenter = null;
            }
        };

        _coordTimer.Start();
    }
    #endregion

    #region Showing and hiding elements
    #region Path to target ellipse
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
    #endregion

    #region Path in target ellipse
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
    #endregion

    #region Full path ellipse
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
    #endregion

    #region Cursor
    private bool _isCursorVisible = true;

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
    #endregion

    #region Target
    private bool _isTargetVisible = true;

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
    #endregion
    #endregion

    #region Sidebar animations
    private Brush _detectionAreaColor = Brushes.Transparent;
    private void MouseDetectionArea_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _detectionAreaColor = MouseDetectionArea.Background;
        MouseDetectionArea.Background = Brushes.Transparent;

        var animation = new DoubleAnimation
        {
            To = 0,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        SidebarTransform.BeginAnimation(TranslateTransform.XProperty, animation);

        DimOverlay.Visibility = Visibility.Visible;
    }

    private void Sidebar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        MouseDetectionArea.Background = _detectionAreaColor;

        var animation = new DoubleAnimation
        {
            To = Sidebar.ActualWidth + 100,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        SidebarTransform.BeginAnimation(TranslateTransform.XProperty, animation);

        DimOverlay.Visibility = Visibility.Hidden;
    }
    #endregion
}
