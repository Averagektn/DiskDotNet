using Disk.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for MapCreatorView.xaml
    /// </summary>
    public partial class MapCreatorView : UserControl
    {
        private MapCreatorViewModel? ViewModel => DataContext as MapCreatorViewModel;

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
            ViewModel?.SelectTarget(mousePos);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(sender as UIElement);
                if (AllowedArea.FillContains(mousePos))
                {
                    ViewModel?.MoveTarget(mousePos);
                }
            }
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            ViewModel?.RemoveTarget(PaintArea.Children, mousePos);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AllowedArea.RadiusX = ActualWidth / 2;
            AllowedArea.RadiusY = ActualHeight / 2;
            AllowedArea.Center = new(ActualWidth / 2, ActualHeight / 2);

            ViewModel?.ScaleTargets(RenderSize);

            const int iniFontSize = 15;
            const int iniHeight = 400;
            const int iniWidth = 800;
            double heightScale = e.NewSize.Height / iniHeight;
            double widthScale = e.NewSize.Width / iniWidth;
            Menu.FontSize = iniFontSize * double.Min(widthScale, heightScale);
            Menu.Height = Menu.FontSize + 10;
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
