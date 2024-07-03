using Disk.Service.Implementation;
using Disk.ViewModel;
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
        private int PaintPanelCenterX => (int)PaintPanelSize.Width / 2;
        private int PaintPanelCenterY => (int)PaintPanelSize.Height / 2;

        private List<IScalable> Scalables { get; set; } = [];

        public SessionResultView()
        {
            InitializeComponent();

            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel?.Converter.Scale(PaintPanelSize);
            Scalables.ForEach(s => s.Scale(PaintPanelSize));
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            OnSizeChanged(sender, null!);
            SelectionChanged(sender, null!);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PaintArea.Children.Clear();

            if (ViewModel is not null)
            {
                var figures = ViewModel.GetPathAndRose(PaintPanelSize);
                foreach (var figure in figures)
                {
                    Scalables.Add(figure);

                    figure.Scale(PaintPanelSize);
                    figure.Draw(PaintArea);
                }
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

            PaintArea.Children.Clear();

            int targetNum = 0;
            var target = DrawableFabric.GetIniProgressTarget(new(0, 0));

            var user = DrawableFabric.GetIniUser(string.Empty);
            user.Move(new(PaintPanelCenterX, PaintPanelCenterY));

            Scalables.Add(user);
            Scalables.Add(target);
            Scalables.ForEach(item => item.Scale(PaintPanelSize));
            target.Move(ViewModel.Converter.ToWnd_FromRelative(ViewModel.TargetCenters[targetNum++]));

            target.Draw(PaintArea);
            user.Draw(PaintArea);

            var shotTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.ShotTime)
            };
            shotTimer.Tick += (_, _) =>
            {
                user.Shot();
            };

            var enumerator = ViewModel.FullPath.GetEnumerator();
            var moveTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.MoveTime)
            };
            moveTimer.Tick += (_, _) =>
            {
                if (enumerator.MoveNext())
                {
                    user.Move(ViewModel.Converter.ToWndCoord(enumerator.Current.Point));
                    if (enumerator.Current.IsNewTarget && targetNum < ViewModel.TargetCenters.Count)
                    {
                        target.Move(ViewModel.Converter.ToWnd_FromRelative(ViewModel.TargetCenters[targetNum++]));
                    }
                }
                else
                {
                    moveTimer.Stop();
                    shotTimer.Stop();
                }
            };

            shotTimer.Start();
            moveTimer.Start();
        }
    }
}
