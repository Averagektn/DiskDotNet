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
        private readonly MapCreatorViewModel _viewModel = new();

        public MapCreatorView()
        {
            InitializeComponent();

            Unloaded += OnClose;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseRightButtonDown += OnMouseRightButtonDown;
            MouseDoubleClick += OnMouseDoubleClick;
            MouseMove += OnMouseMove;
            SizeChanged += OnSizeChanged;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            _viewModel.SelectTarget(mousePos);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(sender as UIElement);
                if (AllowedArea.FillContains(mousePos))
                {
                    _viewModel.MoveTarget(mousePos);
                }
            }
        }

        private void OnClose(object? sender, RoutedEventArgs e) => _viewModel.SaveMap(ActualWidth, ActualHeight);

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            _viewModel.RemoveTarget(PaintArea.Children, mousePos);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AllowedArea.RadiusX = ActualWidth / 2;
            AllowedArea.RadiusY = ActualHeight / 2;
            AllowedArea.Center = new(ActualWidth / 2, ActualHeight / 2);

            _viewModel.ScaleTargets(RenderSize);
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            if (e.ChangedButton == MouseButton.Left && AllowedArea.FillContains(new Point(x, y)))
            {
                _viewModel.AddTarget(mousePos, RenderSize, PaintArea);
            }
        }
    }
}
