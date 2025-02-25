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
        private Target? _selectedTarget = null;

        public MapCreatorView()
        {
            InitializeComponent();

            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseRightButtonDown += OnMouseRightButtonDown;
            MouseDoubleClick += OnMouseDoubleClick;
            MouseMove += OnMouseMove;
            SizeChanged += OnSizeChanged;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            _selectedTarget = ViewModel?.GetTarget(mousePos);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var window = Window.GetWindow(this);
            var converter = new Converter((int)window.ActualWidth, (int)window.ActualHeight, Settings.Default.XMaxAngle * 2, Settings.Default.YMaxAngle * 2);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(sender as UIElement);
                var x = (int)mousePos.X;
                var y = (int)mousePos.Y;
                if (AllowedArea.FillContains(mousePos))
                {
                    var clickPoint = new Point2D<int>(x, y);

                    var point = converter.ToAngle_FromWnd(new(x, y));
                    // replace
                    window.Title = $"{point.X:f2}; {point.Y:f2}";
                    _selectedTarget?.Move(clickPoint);
                }
                else
                {
                    var center = new Point2D<int>((int)AllowedArea.Bounds.Width / 2, (int)AllowedArea.Bounds.Height / 2);
                    var radiusX = AllowedArea.Bounds.Width / 2;
                    var radiusY = AllowedArea.Bounds.Height / 2;

                    double normalizedX = (x - center.X) / radiusX;
                    double normalizedY = (y - center.Y) / radiusY;

                    double length = Math.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);

                    double scale = 1 / length;
                    int nearestX = (int)(center.X + normalizedX * radiusX * scale);
                    int nearestY = (int)(center.Y + normalizedY * radiusY * scale);

                    _selectedTarget?.Move(new(nearestX, nearestY));
                }
            }
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            ViewModel?.RemoveTarget(mousePos);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AllowedArea.RadiusX = ActualWidth / 2;
            AllowedArea.RadiusY = ActualHeight / 2;
            AllowedArea.Center = new(ActualWidth / 2, ActualHeight / 2);

            ViewModel?.ScaleTargets(RenderSize);
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            if (e.ChangedButton == MouseButton.Left && AllowedArea.FillContains(new Point(x, y)))
            {
                ViewModel?.AddTarget(mousePos, RenderSize, PaintArea);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.SaveMap(PaintArea.ActualWidth, PaintArea.ActualHeight);
        }
    }
}
