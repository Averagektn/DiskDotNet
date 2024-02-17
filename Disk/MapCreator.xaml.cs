using Disk.Visual.Impl;
using System.Windows;
using System.Windows.Input;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MapCreator.xaml
    /// </summary>
    public partial class MapCreator : Window
    {
        private Target? _target;

        public MapCreator()
        {
            InitializeComponent();

            MouseRightButtonDown += OnMouseRightButtonDown;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _target?.Scale(RenderSize);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _target = new Target(new(0, 0), Config.Config.Default.TARGET_INI_RADIUS, RenderSize);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);

            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            _target?.Move(new(x, y));
            _target?.Draw(PaintArea);
        }
    }
}
