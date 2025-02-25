using Disk.Calculations.Impl.Converters;
using Disk.Service.Implementation;
using Disk.ViewModel;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for SessionResultView.xaml
    /// </summary>
    public partial class SessionResultView : UserControl
    {
        private DispatcherTimer MoveTimer = new(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(Settings.MoveTime)
        };

        private static Settings Settings => Settings.Default;
        private SessionResultViewModel? ViewModel => DataContext as SessionResultViewModel;

        private Size PaintPanelSize => PaintArea.RenderSize;

        private List<IScalable> Scalables { get; set; } = [];

        private readonly User _user = null!; //DrawableFabric.GetIniUser(string.Empty);
        private readonly Target _target = null!;// DrawableFabric.GetIniProgressTarget(new(0, 0));
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
            Scalables.ForEach(s => s.Scale());
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Scalables.AddRange([_user, _target]);

            OnSizeChanged(sender, null!);
            SelectionChanged(sender, null!);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel is null)
            {
                return;
            }

            if (!IsReply)
            {
                _user.Move(ViewModel.UserCenter);
                _target.Move(ViewModel.TargetCenter);
            }

            PaintArea.Children.Clear();
            _target.Draw();
            _user.Draw();

            var figures = ViewModel.GetPathAndRose(PaintPanelSize, PaintArea);
            foreach (var figure in figures)
            {
                Scalables.Add(figure);

                //figure.Draw(PaintArea);
                figure.Scale();
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

            Scalables.ForEach(item => item.Scale());

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
                        _target.Move(ViewModel.Converter.ToWnd_FromRelative(ViewModel.TargetCenters[selectedIndex]));
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
}
