using Disk.Data.Impl;
using Disk.Navigators;
using Disk.Stores;
using Disk.Stores.Interface;
using Disk.ViewModel.Common.ViewModels;
using Disk.Visual.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel
{
    public class MapCreatorViewModel(ModalNavigationStore modalNavigationStore) : ObserverViewModel
    {
        private static int IniWidth => Settings.Default.IniScreenWidth;
        private static int IniHeight => Settings.Default.IniScreenHeight;

        private readonly List<NumberedTarget> _targets = [];
        private Target? _movingTarget;

        public void SelectTarget(Point mousePos)
        {
            var x = (int)mousePos.X;
            var y = (int)mousePos.Y;

            _movingTarget = _targets.FindLast(target => target.Contains(new Point2D<int>(x, y)));
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
                var map = _targets
                    .Select(t => new Point2D<float>
                    (
                        (float)(t.Center.X / actualWidth),
                        (float)(t.Center.Y / actualHeight))    
                    )    
                    .ToList();

                MapNamePickerNavigator.Navigate(modalNavigationStore, map);
            }

            IniNavigationStore.Close();
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
                Settings.Default.IniTargetRadius,
                new Size(IniWidth, IniHeight),
                _targets.Count + 1
               );
    }
}