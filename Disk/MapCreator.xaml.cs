using Disk.Data.Impl;
using Disk.Visual.Impl;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MapCreator.xaml
    /// </summary>
    public partial class MapCreator : Window
    {
        public int MapId { get; set; } = 1;

        private readonly List<Target> _targets = [];

        public MapCreator()
        {
            InitializeComponent();

            Closing += OnClose;
            MouseRightButtonDown += OnMouseRightButtonDown;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            SizeChanged += OnSizeChanged;
        }

        private void OnClose(object? sender, CancelEventArgs e)
        {
            using var writer = Logger.GetLogger($"maps\\map_{MapId}.map");

            foreach (var target in _targets)
            {
                writer.LogLn(new Point2D<float>(
                    (float)(target.Center.X / ActualWidth),
                    (float)(target.Center.Y / ActualHeight)));
            }
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            var removableTagets = _targets.Where(target => target.Contains(new(x, y)));
            foreach (var target in removableTagets)
            {
                target.Remove(PaintArea.Children);
            }
            _targets.RemoveAll(target => target.Contains(new(x, y)));
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AllowedArea.RadiusX = ActualWidth / 2;
            AllowedArea.RadiusY = ActualHeight / 2;
            AllowedArea.Center = new(ActualWidth / 2, ActualHeight / 2);

            foreach (var target in _targets)
            {
                target?.Draw(PaintArea);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;
            if (AllowedArea.FillContains(new Point(x, y)))
            {
                var newTarget = new Target(new(x, y), Config.Config.Default.TARGET_INI_RADIUS, RenderSize);
                newTarget.Draw(PaintArea);
                _targets.Add(newTarget);
            }
        }
    }
}
