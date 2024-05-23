using Disk.Data.Impl;
using Disk.Visual.Impl;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Settings = Disk.Properties.Config.Config;

namespace Disk
{
    /// <summary>
    /// Interaction logic for MapCreator.xaml
    /// </summary>
    public partial class MapCreator : Window
    {
        public int MapId { get; set; } = Settings.Default.MAP_ID;

        private static int IniWidth => Settings.Default.SCREEN_INI_WIDTH;
        private static int IniHeight => Settings.Default.SCREEN_INI_HEIGHT;

        private readonly List<NumberedTarget> _targets = [];

        private Target? _movingTarget;

        public MapCreator()
        {
            InitializeComponent();

            Closing += OnClose;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseRightButtonDown += OnMouseRightButtonDown;
            MouseDoubleClick += OnMouseDoubleClick;
            MouseMove += OnMouseMove;
            SizeChanged += OnSizeChanged;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            _movingTarget = _targets.Find(target => target.Contains(new Point2D<int>(x, y)));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(sender as UIElement);
                var x = (int)mousePos.X;
                var y = (int)mousePos.Y;
                var clickPoint = new Point2D<int>(x, y);

                _movingTarget?.Move(clickPoint);
            }
        }

        private void OnClose(object? sender, CancelEventArgs e)
        {
            if (_targets.Count != 0)
            {
                using var writer = Logger.GetLogger($"maps\\map_{MapId++}.map");

                foreach (var target in _targets)
                {
                    writer.LogLn(new Point2D<float>(
                        (float)(target.Center.X / ActualWidth),
                        (float)(target.Center.Y / ActualHeight)));
                }

                Settings.Default.MAP_ID = MapId;
                Settings.Default.Save();
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

            for (int i = 0; i < _targets.Count; i++)
            {
                _targets[i].UpdateNumber(i + 1);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AllowedArea.RadiusX = ActualWidth / 2 - 8;
            AllowedArea.RadiusY = ActualHeight / 2 - 20;
            AllowedArea.Center = new(ActualWidth / 2 - 8, ActualHeight / 2 - 20);

            foreach (var target in _targets)
            {
                target?.Scale(RenderSize);
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(sender as UIElement);
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            if (e.ChangedButton == MouseButton.Left && AllowedArea.FillContains(new Point(x, y)))
            {
                var newTarget = GetIniCoordTarget(mousePos.X, mousePos.Y);
                newTarget.Scale(RenderSize);
                newTarget.Draw(PaintArea);
                _targets.Add(newTarget);
            }
        }

        private NumberedTarget GetIniCoordTarget(double actualX, double actualY) =>
            new(
                new Point2D<int>(
                    (int)(actualX / RenderSize.Width * IniWidth),
                    (int)(actualY / RenderSize.Height * IniHeight)
                   ),
                Settings.Default.TARGET_INI_RADIUS,
                new Size(IniWidth, IniHeight),
                _targets.Count + 1
               );
    }
}
