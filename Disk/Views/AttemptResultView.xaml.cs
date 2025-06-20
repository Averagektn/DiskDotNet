﻿using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Disk.Calculations.Implementations.Converters;
using Disk.Data.Impl;
using Disk.Services.Implementations;
using Disk.ViewModels;
using Disk.Visual.Implementations;
using Disk.Visual.Interfaces;

using Settings = Disk.Properties.Config.Config;

namespace Disk.Views;

public partial class AttemptResultView : UserControl
{
    private AttemptResultViewModel? ViewModel => DataContext as AttemptResultViewModel;
    private Size PaintPanelSize => PaintArea.RenderSize;
    private Converter? Converter => ViewModel?.Converter;

    private Cursor? _cursor;
    private ITarget? _target;
    private int _currentIndex = -1;

    private IStaticFigure? _pathToTarget;
    private IStaticFigure? _pathInTarget;
    private IStaticFigure? _diagram;

    private readonly List<IStaticFigure> _pathToTargetEllipses = [];
    private readonly List<IStaticFigure> _pathInTargetEllipses = [];
    private readonly List<IStaticFigure> _fullPathEllipses = [];

    private readonly Size _iniScreenSize = new(_settings.IniScreenWidth, _settings.IniScreenHeight);

    private static readonly Settings _settings = Settings.Default;
    private Thread? _coordThread;

    private bool _isReply;
    private bool IsReply
    {
        get => _isReply;
        set
        {
            _isReply = value;
            FastForwardSlider.IsEnabled = !_isReply;
            if (ViewModel is not null)
            {
                ViewModel.IsStopEnabled = _isReply;
            }
        }
    }

    public AttemptResultView()
    {
        InitializeComponent();

        SizeChanged += (_, _) => Converter?.Scale(PaintPanelSize);
        Loaded += AttemptResultView_Loaded;
        Loaded += (_, _) => SidebarTransform.X = Sidebar.ActualWidth + 100;

        Unloaded += StopReply;
    }

    private void AttemptResultView_Loaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        _cursor = DrawableFabric.GetIniCursor(string.Empty, PaintArea);
        _target = DrawableFabric.GetIniProgressTarget(string.Empty, new(0, 0), PaintArea);

        _target.Move(ViewModel.TargetCenter);

        _target.Draw();
        _cursor.Draw();
    }

    private bool _isRenderAdded = false;
    private void SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        _diagram?.Remove();
        _pathToTarget?.Remove();
        _pathInTarget?.Remove();

        _diagram = ViewModel.GetGraph(GraphArea);
        (_pathToTarget, _pathInTarget) = ViewModel.GetPath(PathArea);

        if (DiagramCheckBox.IsChecked ?? false)
        {
            _diagram?.Draw();
        }
        if (PathToTargetCheckBox.IsChecked ?? false)
        {
            _pathToTarget?.Draw();
        }
        if (PathInTargetCheckBox.IsChecked ?? false)
        {
            _pathInTarget?.Draw();
        }

        if (_currentIndex == ViewModel.SelectedIndex)
        {
            return;
        }
        _currentIndex = ViewModel.SelectedIndex;

        _target?.Move(ViewModel.TargetCenter);

        const int MinEllipsePointsCount = 10;
        if (ViewModel.CurrPathInTarget.Count > MinEllipsePointsCount && ViewModel.CurrPathToTarget.Count > MinEllipsePointsCount)
        {
            RecalculateEllipse(_fullPathEllipses, [.. ViewModel.CurrPathToTarget, .. ViewModel.CurrPathInTarget],
                Brushes.MediumPurple, Brushes.Purple);
            if (FullPathEllipseCheckBox.IsChecked ?? false)
            {
                _fullPathEllipses.ForEach(e => e.Draw());
            }
        }
        if (ViewModel.CurrPathInTarget.Count > MinEllipsePointsCount)
        {
            RecalculateEllipse(_pathInTargetEllipses, [.. ViewModel.CurrPathInTarget], Brushes.CadetBlue, Brushes.Blue);
            if (PathInTargetEllipseCheckBox.IsChecked ?? false)
            {
                _pathInTargetEllipses.ForEach(e => e.Draw());
            }
        }
        if (ViewModel.CurrPathToTarget.Count > MinEllipsePointsCount)
        {
            RecalculateEllipse(_pathToTargetEllipses, [.. ViewModel.CurrPathToTarget], Brushes.LawnGreen, Brushes.Green);
            if (PathToTargetEllipseCheckBox.IsChecked ?? false)
            {
                _pathToTargetEllipses.ForEach(e => e.Draw());
            }
        }

        if (!_isRenderAdded)
        {
            CompositionTarget.Rendering += OnRender;
            _isRenderAdded = true;
        }
    }

    #region Ellipse and ConvexHull creation
    private void RecalculateEllipse(List<IStaticFigure> ellipseList, List<Point2D<float>> points, Brush fill, Brush bound)
    {
        ellipseList.ForEach(e => e.Remove());
        ellipseList.Clear();

        IStaticFigure convexHull = CreateConvexHull(points, bound, fill);
        IStaticFigure boundingEllipse = CreateBoundingEllipse(points, bound);

        ellipseList.AddRange([convexHull, boundingEllipse]);
    }

    private IStaticFigure CreateConvexHull(List<Point2D<float>> points, Brush borderColor, Brush fillColor)
    {
        ConvexHull ch;

        try
        {
            Converter converter = DrawableFabric.GetIniConverter();
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
        Converter converter = DrawableFabric.GetIniConverter();
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

    private IEnumerator<Point2D<float>>? _enumerator;

    private void OnRender(object? sender, EventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        //_cursor?.Move(ViewModel.CursorCenter);
        _cursor?.MoveSmooth(ViewModel.CursorCenter);
    }


    private void StopReply(object sender, RoutedEventArgs e)
    {
        IsReply = false;
        _coordThread?.Join();
    }

    private void ReplyClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        _enumerator = ViewModel.FullPath.GetEnumerator();
        int intervalMs = ViewModel.CurrentAttempt.SamplingInterval;

        _coordThread?.Join();
        _coordThread = new Thread(_ =>
        {
            var stopwatch = Stopwatch.StartNew();
            long nextTick = stopwatch.ElapsedMilliseconds;

            while (IsReply)
            {
                long now = stopwatch.ElapsedMilliseconds;

                if (now >= nextTick)
                {
                    if (!_enumerator.MoveNext())
                    {
                        _ = Dispatcher.Invoke(() => IsReply = false);
                    }

                    nextTick += intervalMs;
                }
            }
        })
        {
            Priority = ThreadPriority.Highest,
            IsBackground = true,
        };
        IsReply = true;
        _coordThread.Start();
    }
    #endregion

    #region Showing and hiding elements

    #region Path to target ellipse
    public void ShowPathToTargetEllipse(object sender, RoutedEventArgs e)
    {
        _pathToTargetEllipses.ForEach(e =>
        {
            e.Draw();
            e.Scale();
        });
    }
    public void HidePathToTargetEllipse(object sender, RoutedEventArgs e)
    {
        _pathToTargetEllipses.ForEach(e => e.Remove());
    }
    #endregion

    #region Path in target ellipse
    public void ShowPathInTargetEllipse(object sender, RoutedEventArgs e)
    {
        _pathInTargetEllipses.ForEach(e =>
        {
            e.Draw();
            e.Scale();
        });
    }
    public void HidePathInTargetEllipse(object sender, RoutedEventArgs e)
    {
        _pathInTargetEllipses.ForEach(e => e.Remove());
    }
    #endregion

    #region Full path ellipse
    public void ShowFullPathEllipse(object sender, RoutedEventArgs e)
    {
        _fullPathEllipses.ForEach(e =>
        {
            e.Draw();
            e.Scale();
        });
    }
    public void HideFullPathEllipse(object sender, RoutedEventArgs e)
    {
        _fullPathEllipses.ForEach(e => e.Remove());
    }
    #endregion

    #region Cursor
    private void ShowCursor(object sender, RoutedEventArgs e)
    {
        _cursor?.Draw();
        _cursor?.Scale();
    }
    private void HideCursor(object sender, RoutedEventArgs e)
    {
        _cursor?.Remove();
    }
    #endregion

    #region Target
    private void ShowTarget(object sender, RoutedEventArgs e)
    {
        _target?.Draw();
        _target?.Scale();
    }
    private void HideTarget(object sender, RoutedEventArgs e)
    {
        _target?.Remove();
    }
    #endregion

    #region Diagram
    private void DiagramCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _diagram?.Draw();
        _diagram?.Scale();
    }
    private void DiagramCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _diagram?.Remove();
    }
    #endregion

    #region Path to target
    private void PathToTargetCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        _pathToTarget?.Draw();
        _pathInTarget?.Scale();
    }
    private void PathToTargetCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        _pathToTarget?.Remove();
    }
    #endregion

    #region Path in target
    private void PathInTargetCheckbox_Checked(object sender, RoutedEventArgs e)
    {
        _pathInTarget?.Draw();
        _pathInTarget?.Scale();
    }
    private void PathInTargetCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        _pathInTarget?.Remove();
    }
    #endregion

    #endregion

    #region Sidebar animations
    private Brush _detectionAreaColor = Brushes.Transparent;

    private void MouseDetectionArea_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _detectionAreaColor = MouseDetectionArea.Background;
        MouseDetectionArea.Background = Brushes.Transparent;

        DoubleAnimation animation = new()
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

        DoubleAnimation animation = new()
        {
            To = Sidebar.ActualWidth + 100,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        SidebarTransform.BeginAnimation(TranslateTransform.XProperty, animation);

        DimOverlay.Visibility = Visibility.Hidden;
    }
    #endregion
}
