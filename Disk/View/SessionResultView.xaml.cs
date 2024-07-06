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
        private static Settings Settings => Settings.Default;
        private SessionResultViewModel? ViewModel => DataContext as SessionResultViewModel;

        private Size PaintPanelSize => PaintArea.RenderSize;

        private List<IScalable> Scalables { get; set; } = [];

        private static readonly User _user = DrawableFabric.GetIniUser(string.Empty);
        private static readonly Target _target = DrawableFabric.GetIniProgressTarget(new(0, 0));
        private Converter? Converter => ViewModel?.Converter;
        private bool _isReply;

        public SessionResultView()
        {
            InitializeComponent();

            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Converter?.Scale(PaintPanelSize);
            Scalables.ForEach(s => s.Scale(PaintPanelSize));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Scalables.AddRange([_user, _target]);

            _target.Move(ViewModel!.TargetCenter);
            _user.Move(ViewModel!.UserCenter);

            PaintArea.Children.Clear();
            _target.Draw(PaintArea);
            _user.Draw(PaintArea);

            OnSizeChanged(sender, null!);
            SelectionChanged(sender, null!);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel is null)
            {
                return;
            }

            if (!_isReply)
            {
                _user.Move(ViewModel.UserCenter);
                _target.Move(ViewModel.TargetCenter);
            }

            PaintArea.Children.Clear();
            _target.Draw(PaintArea);
            _user.Draw(PaintArea);

            var figures = ViewModel.GetPathAndRose(PaintPanelSize);
            foreach (var figure in figures)
            {
                Scalables.Add(figure);

                figure.Draw(PaintArea);
                figure.Scale(PaintPanelSize);
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

            _isReply = true;

            Scalables.ForEach(item => item.Scale(PaintPanelSize));

            var enumerator = ViewModel.FullPath.GetEnumerator();
            var moveTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.MoveTime)
            };
            moveTimer.Tick += (_, _) =>
            {
                if (enumerator.MoveNext())
                {
                    _user.Move(ViewModel.Converter.ToWndCoord(enumerator.Current.Point));

                    if (enumerator.Current.IsNewTarget && ++ViewModel.SelectedIndex < ViewModel.TargetCenters.Count)
                    {
                        _target.Move(ViewModel.Converter.ToWnd_FromRelative(ViewModel.TargetCenters[ViewModel.SelectedIndex]));
                    }
                }
                else
                {
                    StopTimer(sender, e);
                    Unloaded -= StopTimer;
                }
            };

            Unloaded += StopTimer;

            void StopTimer(object sender, RoutedEventArgs e)
            {
                if (moveTimer.IsEnabled)
                {
                    _isReply = false;
                    moveTimer.Stop();
                }
            }

            moveTimer.Start();
        }
    }
}
