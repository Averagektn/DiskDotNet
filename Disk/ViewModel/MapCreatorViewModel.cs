using Disk.Data.Impl;
using Disk.ViewModel.Common;
using Disk.Visual.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class MapCreatorViewModel : ObserverViewModel
    {
        public int MapId { get; set; } = Settings.Default.MAP_ID;

        private static int IniWidth => Settings.Default.SCREEN_INI_WIDTH;
        private static int IniHeight => Settings.Default.SCREEN_INI_HEIGHT;

        private readonly List<NumberedTarget> _targets = [];

        private Target? _movingTarget;

        public void SelectTarget(Point mousePos)
        {
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            _movingTarget = _targets.Find(target => target.Contains(new Point2D<int>(x, y)));
        }

        public void MoveTarget(Point mousePos)
        {
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;
            var clickPoint = new Point2D<int>(x, y);

            _movingTarget?.Move(clickPoint);
        }

        public void SaveMap(double actualWidth, double actualHeight)
        {
            if (_targets.Count != 0)
            {
                using var writer = Logger.GetLogger($"maps\\map_{MapId++}.map");

                foreach (var target in _targets)
                {
                    writer.LogLn(new Point2D<float>(
                        (float)(target.Center.X / actualWidth),
                        (float)(target.Center.Y / actualHeight)));
                }

                Settings.Default.MAP_ID = MapId;
                Settings.Default.Save();
            }
        }

        public void RemoveTarget(UIElementCollection screenTargets, Point mousePos)
        {
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            var removableTagets = _targets.Where(target => target.Contains(new(x, y)));
            foreach (var target in removableTagets)
            {
                target.Remove(screenTargets);
            }
            _ = _targets.RemoveAll(target => target.Contains(new(x, y)));

            for (int i = 0; i < _targets.Count; i++)
            {
                _targets[i].UpdateNumber(i + 1);
            }
        }

        public void ScaleTargets(Size size)
        {
            foreach (var target in _targets)
            {
                target?.Scale(size);
            }
        }

        public void AddTarget(Point mousePos, Size renderSize, IAddChild paintArea)
        {
            var newTarget = GetIniCoordTarget(renderSize, mousePos.X, mousePos.Y);
            newTarget.Scale(renderSize);
            newTarget.Draw(paintArea);
            _targets.Add(newTarget);
        }

        private NumberedTarget GetIniCoordTarget(Size renderSize, double actualX, double actualY) =>
            new(
                new Point2D<int>(
                    (int)(actualX / renderSize.Width * IniWidth),
                    (int)(actualY / renderSize.Height * IniHeight)
                   ),
                Settings.Default.TARGET_INI_RADIUS,
                new Size(IniWidth, IniHeight),
                _targets.Count + 1
               );
    }
}