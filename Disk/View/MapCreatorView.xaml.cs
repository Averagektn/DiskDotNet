using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.ViewModel;
using Disk.Visual.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for MapCreatorView.xaml
    /// </summary>
    public partial class MapCreatorView : UserControl
    {
        private MapCreatorViewModel? ViewModel => DataContext as MapCreatorViewModel;
        private NumberedTarget? _selectedTarget = null;
        private static int IniWidth = Settings.Default.IniScreenWidth;
        private static int IniHeight = Settings.Default.IniScreenHeight;
        private static float AngleWidth => Settings.Default.XMaxAngle * 2;
        private static float AngleHeight => Settings.Default.YMaxAngle * 2;

        private readonly List<NumberedTarget> _targets = [];

        private Converter? _converter = null;

        public MapCreatorView()
        {
            InitializeComponent();

            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseRightButtonDown += OnMouseRightButtonDown;
            MouseDoubleClick += OnMouseDoubleClick;
            MouseMove += OnMouseMove;
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
            LayoutUpdated += OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            Canvas.SetLeft(MaxX, PaintArea.ActualWidth - 2 - MaxX.ActualWidth);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _converter = new Converter((int)PaintArea.ActualWidth, (int)PaintArea.ActualHeight, AngleWidth, AngleHeight);
            IniWidth = (int)PaintArea.ActualWidth;
            IniHeight = (int)PaintArea.ActualHeight;

            MaxX.Text = $"X:{Settings.Default.XMaxAngle:f1}";
            MaxY.Text = $"Y: {Settings.Default.YMaxAngle:f1}";
        }

        private bool _isMoveTriggered = false;
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMoveTriggered = false;
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            var prevTarget = _selectedTarget;
            _selectedTarget = _targets.FindLast(target => target.Contains(new Point2D<int>(x, y)));

            prevTarget?.HideAngles();
            _selectedTarget?.ShowAngles();

            _isMoveTriggered = _selectedTarget is not null;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _isMoveTriggered)
            {
                var mousePos = e.GetPosition(sender as UIElement);
                var x = (int)mousePos.X;
                var y = (int)mousePos.Y;
                if (AllowedArea.FillContains(mousePos))
                {
                    var clickPoint = new Point2D<int>(x, y);
                    _selectedTarget?.Move(clickPoint);
                }
                else
                {
                    var center = new Point2D<int>((int)AllowedArea.Bounds.Width / 2, (int)AllowedArea.Bounds.Height / 2);
                    var radiusX = AllowedArea.Bounds.Width / 2;
                    var radiusY = AllowedArea.Bounds.Height / 2;

                    double normalizedX = (x - center.X) / radiusX;
                    double normalizedY = (y - center.Y) / radiusY;

                    double length = Math.Sqrt((normalizedX * normalizedX) + (normalizedY * normalizedY));

                    double scale = 1 / length;
                    int nearestX = (int)(center.X + (normalizedX * radiusX * scale));
                    int nearestY = (int)(center.Y + (normalizedY * radiusY * scale));

                    _selectedTarget?.Move(new(nearestX, nearestY));
                }
            }
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            var target = _targets.FindLast(target => target.Contains(new(x, y)));
            if (target is not null)
            {
                _ = _targets.Remove(target);
                target.Remove();

                for (int i = 0; i < _targets.Count; i++)
                {
                    _targets[i].UpdateNumber(i + 1);
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AllowedArea.RadiusX = ActualWidth / 2;
            AllowedArea.RadiusY = ActualHeight / 2;
            AllowedArea.Center = new(ActualWidth / 2, ActualHeight / 2);

            _converter?.Scale(new(ActualWidth, ActualHeight));
            _targets.ForEach(target => target.Scale());

            Canvas.SetLeft(MaxX, PaintArea.ActualWidth - 2 - MaxX.ActualWidth);
            Canvas.SetTop(MaxX, PaintArea.ActualHeight / 2 + 2);

            Canvas.SetLeft(MaxY, PaintArea.ActualWidth / 2 + 2);
            Canvas.SetTop(MaxY, 2);
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            if (e.ChangedButton == MouseButton.Left && AllowedArea.FillContains(new Point(x, y)))
            {
                var target = GetIniCoordTarget(mousePos.X, mousePos.Y);
                target.Scale();
                target.Draw();
                _targets.Add(target);
            }
        }

        private NumberedTarget GetIniCoordTarget(double actualX, double actualY)
        {
            return new(
                center: new Point2D<int>
                (
                    (int)(actualX / PaintArea.ActualWidth * IniWidth),
                    (int)(actualY / PaintArea.ActualHeight * IniHeight)
                ),
                radius: Settings.Default.IniTargetRadius,
                parent: PaintArea,
                number: _targets.Count + 1,
                iniSize: new(IniWidth, IniHeight),
                converter: _converter!
            );
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.SaveMap(_targets);
        }
    }
}
