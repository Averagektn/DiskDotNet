using Disk.Calculations.Impl.Converters;
using Disk.Service.Implementation;
using Disk.ViewModel;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View;

public partial class SessionResultView : UserControl
{
    private DispatcherTimer MoveTimer = new(DispatcherPriority.Normal)
    {
        Interval = TimeSpan.FromMilliseconds(Settings.ShotTime)
    };
    private IUser _user = null!;
    private ITarget _target = null!;

    //private List<IScalable> Scalables { get; set; } = [];
    private static Settings Settings => Settings.Default;
    private SessionResultViewModel? ViewModel => DataContext as SessionResultViewModel;
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

    public SessionResultView()
    {
        InitializeComponent();

        SizeChanged += OnSizeChanged;
        Loaded += OnLoaded;
        Unloaded += StopTimer;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Converter?.Scale(PaintPanelSize);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        OnSizeChanged(sender, null!);
        SelectionChanged(sender, null!);
    }

    private List<IStaticFigure> _pathAndRose = [];

    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        _target?.Remove();
        _user?.Remove();

        var color = new SolidColorBrush(Color.FromRgb(Settings.UserColor.R, Settings.UserColor.G, Settings.UserColor.B));
        var iniSize = new Size(Settings.IniScreenWidth, Settings.IniScreenHeight);
        _user ??= new User(new(0, 0), ViewModel.CurrentSession.CursorRadius * 5, 0, color, PaintArea, iniSize);
        //_user ??= DrawableFabric.GetIniUser(string.Empty, PaintArea);
        //_target ??= DrawableFabric.GetIniProgressTarget("", new(0, 0), PaintArea);
        _target ??= new Target(new(0, 0), ViewModel.CurrentSession.TargetRadius * 5, PaintArea, iniSize);

        _target.Draw();
        _pathAndRose.ForEach(p => p.Remove());
        _pathAndRose = ViewModel.GetPathAndRose(PaintArea);
        _pathAndRose.ForEach(p => p.Draw());
        _user.Draw();

        if (!IsReply)
        {
            _user.Move(ViewModel.UserCenter);
            _target.Move(ViewModel.TargetCenter);
        }
    }

    private void RadioButtonChecked(object sender, RoutedEventArgs e)
    {
        SelectionChanged(sender, null!);
    }

    private void ReplyClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        IsReply = true;

        var selectedIndex = ViewModel.SelectedIndex;
        var enumerator = ViewModel.FullPath.GetEnumerator();
        MoveTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(Settings.MoveTime)
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
        if (MoveTimer.IsEnabled)
        {
            IsReply = false;
            MoveTimer.Stop();
        }
    }
}
